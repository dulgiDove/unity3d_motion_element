using UnityEngine;

[RequireComponent(typeof(ElementComponent))]
public class PermanentElementProperty : MonoBehaviour
{
    [SerializeField] private ElementType permanentElement = ElementType.None;

    private ElementComponent elementComponent;

    public ElementType PermanentElement => permanentElement;
    public bool IsPermanent => permanentElement != ElementType.None;

    private void Awake()
    {
        elementComponent = GetComponent<ElementComponent>();
    }

    private void Start()
    {
        elementComponent.SetElement(permanentElement);
    }

    private void LateUpdate()
    {
        if (elementComponent.CurrentElement != permanentElement)
            elementComponent.SetElement(permanentElement);
    }
}