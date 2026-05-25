using UnityEngine;

[RequireComponent(typeof(ElementComponent))]
public class ConductiveProperty : MonoBehaviour
{
    [SerializeField] private bool startElectrified;

    private ElementComponent elementComponent;

    public bool IsElectrified => elementComponent.CurrentElement == ElementType.Electricity;

    private void Awake()
    {
        elementComponent = GetComponent<ElementComponent>();
    }

    private void Start()
    {
        if (startElectrified)
            ApplyElectricity();
    }

    public void ApplyElectricity()
    {
        elementComponent.SetElement(ElementType.Electricity);
    }

    public void ClearElectricity()
    {
        if (IsElectrified)
            elementComponent.ClearElement();
    }
}