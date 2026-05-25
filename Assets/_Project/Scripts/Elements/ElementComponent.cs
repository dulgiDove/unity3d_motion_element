using System;
using UnityEngine;

public class ElementComponent : MonoBehaviour
{
    [SerializeField] private ElementType currentElement = ElementType.None;

    public ElementType CurrentElement => currentElement;
    public bool HasElement => currentElement != ElementType.None;

    public event Action<ElementType> OnElementChanged;

    private void Start()
    {
        OnElementChanged?.Invoke(currentElement);
    }

    public void SetElement(ElementType element)
    {
        if (currentElement == element)
            return;

        currentElement = element;
        OnElementChanged?.Invoke(currentElement);
    }

    public void ClearElement()
    {
        SetElement(ElementType.None);
    }
}