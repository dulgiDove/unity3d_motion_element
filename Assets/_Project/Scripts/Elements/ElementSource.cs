using UnityEngine;

public class ElementSource : MonoBehaviour
{
    [SerializeField] private ElementComponent elementComponent;

    public ElementType Element => elementComponent.CurrentElement;
    public bool HasElement => elementComponent.HasElement;

    private void Awake()
    {
        if (elementComponent == null)
            elementComponent = GetComponentInParent<ElementComponent>();
    }
}