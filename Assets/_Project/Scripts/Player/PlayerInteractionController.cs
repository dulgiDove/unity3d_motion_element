using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform dropPoint;

    [Header("Interaction Range")]
    [SerializeField] private float interactForwardOffset = 1.5f;
    [SerializeField] private float interactHeightOffset = 1.0f;
    [SerializeField] private float interactRadius = 1.0f;
    [SerializeField] private LayerMask interactLayerMask = ~0;

    private CarryableObject currentCandidate;
    private CarryableObject carriedObject;

    private void Update()
    {
        UpdateCandidatePrompt();
    }

    private void UpdateCandidatePrompt()
    {
        if (carriedObject != null)
        {
            HideCurrentCandidate();
            return;
        }

        CarryableObject nearest = FindNearestCarryable();

        if (nearest == currentCandidate)
            return;

        HideCurrentCandidate();

        currentCandidate = nearest;

        if (currentCandidate != null)
            currentCandidate.ShowPrompt();
    }

    private void HideCurrentCandidate()
    {
        if (currentCandidate != null)
            currentCandidate.HidePrompt();

        currentCandidate = null;
    }

    private CarryableObject FindNearestCarryable()
    {
        Vector3 center =
            transform.position +
            transform.forward * interactForwardOffset +
            Vector3.up * interactHeightOffset;

        Collider[] hits = Physics.OverlapSphere(
            center,
            interactRadius,
            interactLayerMask,
            QueryTriggerInteraction.Ignore
        );

        CarryableObject nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            CarryableObject carryable = hit.GetComponentInParent<CarryableObject>();

            if (carryable == null)
                continue;

            float distance = Vector3.Distance(transform.position, carryable.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = carryable;
            }
        }

        return nearest;
    }

    public void HandleInteract()
    {
        if (carriedObject != null)
        {
            DropObject();
            return;
        }

        TryPickUpObject();
    }

    private void TryPickUpObject()
    {
        CarryableObject nearest = FindNearestCarryable();

        if (nearest == null)
        {
            return;
        }

        carriedObject = nearest;
        carriedObject.PickUp(holdPoint);
        HideCurrentCandidate();
    }

    private void DropObject()
    {
        Vector3 dropPosition = dropPoint != null
            ? dropPoint.position
            : transform.position + transform.forward * 1.2f;

        Quaternion dropRotation = Quaternion.LookRotation(transform.forward);

        carriedObject.Drop(dropPosition, dropRotation);
        carriedObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center =
            transform.position +
            transform.forward * interactForwardOffset +
            Vector3.up * interactHeightOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, interactRadius);
    }
}