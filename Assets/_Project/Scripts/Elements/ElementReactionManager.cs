using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementReactionManager : MonoBehaviour
{
    public static ElementReactionManager Instance { get; private set; }

    [Header("VFX Prefabs")]
    [SerializeField] private GameObject steamVFXPrefab;
    [SerializeField] private GameObject overloadVFXPrefab;

    [Header("VFX Lifetime")]
    [SerializeField] private float steamDestroyDelay = 2f;
    [SerializeField] private float overloadDestroyDelay = 2f;

    [Header("Overload Damage")]
    [SerializeField] private float overloadDamage = 25f;
    [SerializeField] private float overloadRadius = 3f;
    [SerializeField] private LayerMask overloadDamageLayerMask = ~0;
    [SerializeField] private float overloadCooldown = 0.7f;

    [Header("Electro Charged")]
    [SerializeField] private float electroDuration = 5f;
    [SerializeField] private float electroTickInterval = 0.5f;
    [SerializeField] private float electroDamagePerTick = 5f;
    [SerializeField] private LayerMask electroDamageLayerMask = ~0;

    private float lastOverloadTime = -999f;

    private readonly Dictionary<ElementComponent, Coroutine> electroRoutines = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ExecuteReaction(
        ElementReactionType reaction,
        Vector3 position,
        ElementComponent receiverElement = null,
        Collider receiverCollider = null
    )
    {
        switch (reaction)
        {
            case ElementReactionType.Extinguish:
                SpawnVFX(steamVFXPrefab, position, steamDestroyDelay, "Steam");
                break;

            case ElementReactionType.Overload:
                ExecuteOverload(position);
                break;

            case ElementReactionType.ElectroCharged:
                StartElectroCharged(receiverElement, receiverCollider);
                break;
        }
    }

    private void ExecuteOverload(Vector3 position)
    {
        if (Time.time < lastOverloadTime + overloadCooldown)
            return;

        lastOverloadTime = Time.time;

        SpawnVFX(overloadVFXPrefab, position, overloadDestroyDelay, "Overload");
        ApplyOverloadDamage(position);
    }

    private void ApplyOverloadDamage(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(
            position,
            overloadRadius,
            overloadDamageLayerMask,
            QueryTriggerInteraction.Ignore
        );

        HashSet<Health> damagedTargets = new();

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();

            if (health == null)
                continue;

            if (damagedTargets.Contains(health))
                continue;

            damagedTargets.Add(health);
            health.TakeDamage(overloadDamage);
        }
    }

    private void StartElectroCharged(
        ElementComponent receiverElement,
        Collider receiverCollider
    )
    {
        if (receiverElement == null || receiverCollider == null)
            return;

        if (electroRoutines.TryGetValue(receiverElement, out Coroutine existingRoutine))
            StopCoroutine(existingRoutine);

        Coroutine routine = StartCoroutine(
            ElectroChargedRoutine(receiverElement, receiverCollider)
        );

        electroRoutines[receiverElement] = routine;
    }

    private IEnumerator ElectroChargedRoutine(
        ElementComponent receiverElement,
        Collider receiverCollider
    )
    {
        float timer = electroDuration;

        while (timer > 0f)
        {
            ApplyElectroDamage(receiverCollider);

            yield return new WaitForSeconds(electroTickInterval);

            timer -= electroTickInterval;
        }

        electroRoutines.Remove(receiverElement);
    }

    private void ApplyElectroDamage(Collider receiverCollider)
    {
        Bounds bounds = receiverCollider.bounds;

        Collider[] hits = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            receiverCollider.transform.rotation,
            electroDamageLayerMask,
            QueryTriggerInteraction.Ignore
        );

        HashSet<Health> damagedTargets = new();

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();

            if (health == null)
                continue;

            if (damagedTargets.Contains(health))
                continue;

            damagedTargets.Add(health);
            health.TakeDamage(electroDamagePerTick);

            Debug.Log($"ElectroCharged damaged {health.name} for {electroDamagePerTick}");
        }
    }

    private void SpawnVFX(
        GameObject prefab,
        Vector3 position,
        float destroyDelay,
        string debugName
    )
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{name}: {debugName} VFX Prefab is null");
            return;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        Destroy(instance, destroyDelay);
    }
}