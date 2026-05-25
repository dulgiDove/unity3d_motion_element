using UnityEngine;

[RequireComponent(typeof(ElementComponent))]
public class FlammableProperty : MonoBehaviour
{
    [SerializeField] private bool startBurning;

    private ElementComponent elementComponent;

    public bool IsBurning => elementComponent.CurrentElement == ElementType.Fire;

    private void Awake()
    {
        elementComponent = GetComponent<ElementComponent>();
    }

    private void Start()
    {
        if (startBurning)
            Ignite();
    }

    public void Ignite()
    {
        elementComponent.SetElement(ElementType.Fire);
    }

    public void Extinguish()
    {
        if (IsBurning)
            elementComponent.ClearElement();
    }
}