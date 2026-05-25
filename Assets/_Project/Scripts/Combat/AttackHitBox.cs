using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider hitboxCollider;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackUpwardForce = 0.5f;

    [Header("Hit Stop")]
    [SerializeField] private float hitStopDuration = 0.05f;

    [Header("Camera Shake")]
    [SerializeField] private Vector3 cameraShakeVelocity = new Vector3(1f, 0f, 0f);

    private readonly HashSet<Hurtbox> hitTargets = new();

    private void Awake()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();

        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitboxCollider.enabled)
            return;

        Hurtbox hurtbox = other.GetComponentInParent<Hurtbox>();

        if (hurtbox == null)
            return;

        if (hitTargets.Contains(hurtbox))
            return;

        hitTargets.Add(hurtbox);

        Vector3 hitDirection = other.transform.position - transform.position;
        hitDirection.y = 0f;
        hitDirection.Normalize();

        hurtbox.TakeHit(
            damage,
            hitDirection,
            knockbackForce,
            knockbackUpwardForce
        );

        if (HitStopManager.Instance != null)
            HitStopManager.Instance.PlayHitStop(hitStopDuration);

        if (CameraShakeManager.Instance != null)
            CameraShakeManager.Instance.Shake(cameraShakeVelocity);
    }

    public void EnableHitbox()
    {
        Debug.Log($"{name} Enable Hitbox");
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
        hitTargets.Clear();
    }
}