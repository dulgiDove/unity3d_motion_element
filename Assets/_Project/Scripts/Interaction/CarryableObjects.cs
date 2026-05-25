using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CarryableObject : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject promptUI;

    private Transform originalParent;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        HidePrompt();
    }

    public void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    public void PickUp(Transform holdPoint)
    {
        HidePrompt();

        originalParent = transform.parent;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPosition, Quaternion dropRotation)
    {
        transform.SetParent(originalParent);

        transform.position = dropPosition;
        transform.rotation = dropRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}