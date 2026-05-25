using UnityEngine;

public class PlayerLockOnController : MonoBehaviour
{
    [SerializeField] private float lockOnRange = 12f;

    public Transform CurrentTarget { get; private set; }
    public bool IsLockedOn => CurrentTarget != null;

    public void ToggleLockOn()
    {
        if (IsLockedOn)
        {
            ClearTarget();
            return;
        }

        FindNearestTarget();
    }

    private void FindNearestTarget()
    {
        LockOnTarget[] targets = FindObjectsByType<LockOnTarget>(FindObjectsSortMode.None);

        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (LockOnTarget target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > lockOnRange)
                continue;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target.transform;
            }
        }

        CurrentTarget = nearestTarget;
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }
}