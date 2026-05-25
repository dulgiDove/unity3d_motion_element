using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Health health;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float stopDistance = 1.8f;
    [SerializeField] private float attackRange = 2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 4.5f;

    private EnemyState currentState = EnemyState.Idle;
    private Health targetHealth;
    private float attackCooldownTimer;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
    }

    private void Start()
    {
        if (target != null)
            targetHealth = target.GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }

    private void Update()
    {
        if (target == null)
            return;

        if (currentState == EnemyState.Dead)
            return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        UpdateState();
        TickState();
    }

    private void UpdateState()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > detectionRange)
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (distanceToTarget <= attackRange)
        {
            currentState = EnemyState.Attack;
            return;
        }

        currentState = EnemyState.Chase;
    }

    private void TickState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Chase:
                ChaseTarget();
                break;

            case EnemyState.Attack:
                AttackTarget();
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void ChaseTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= stopDistance)
            return;

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        RotateToward(direction);
    }

    private void AttackTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            RotateToward(direction.normalized);

        if (attackCooldownTimer > 0f)
            return;

        if (targetHealth == null)
            return;

        targetHealth.TakeDamage(attackDamage);
        attackCooldownTimer = attackCooldown;
    }

    private void RotateToward(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleDeath()
    {
        currentState = EnemyState.Dead;
        gameObject.SetActive(false);
    }
}