using UnityEngine;

[RequireComponent(typeof(ElementComponent))]
public class WettableProperty : MonoBehaviour
{
    [SerializeField] private bool startWet;

    private ElementComponent elementComponent;

    public bool IsWet => elementComponent.CurrentElement == ElementType.Water;

    private void Awake()
    {
        elementComponent = GetComponent<ElementComponent>();
    }

    private void Start()
    {
        if (startWet)
            ApplyWater();
    }

    public void ApplyWater()
    {
        elementComponent.SetElement(ElementType.Water);
    }

    public void Dry()
    {
        if (IsWet)
            elementComponent.ClearElement();
    }
}