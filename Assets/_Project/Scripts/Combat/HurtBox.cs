using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private EnemyHitReaction hitReaction;

    private void Awake()
    {
        if (health == null)
            health = GetComponentInParent<Health>();

        if (hitReaction == null)
            hitReaction = GetComponentInParent<EnemyHitReaction>();
    }

    public void TakeHit(
        float damage,
        Vector3 hitDirection,
        float knockbackForce,
        float knockbackUpwardForce
    )
    {
        if (health == null)
        {
            Debug.LogWarning($"{name}: Health is null");
            return;
        }

        Debug.Log($"{name}: TakeHit Damage = {damage}");

        health.TakeDamage(damage);

        if (hitReaction != null)
        {
            hitReaction.PlayHitReaction(
                hitDirection,
                knockbackForce,
                knockbackUpwardForce
            );
        }
    }
}