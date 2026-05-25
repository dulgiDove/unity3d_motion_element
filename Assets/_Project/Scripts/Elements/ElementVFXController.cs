using UnityEngine;

[RequireComponent(typeof(ElementComponent))]
public class ElementVFXController : MonoBehaviour
{
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private GameObject waterVFX;
    [SerializeField] private GameObject electricityVFX;

    private ElementComponent elementComponent;

    private void Awake()
    {
        elementComponent = GetComponent<ElementComponent>();
    }

    private void OnEnable()
    {
        if (elementComponent == null)
            elementComponent = GetComponent<ElementComponent>();

        elementComponent.OnElementChanged += UpdateVFX;
        UpdateVFX(elementComponent.CurrentElement);
    }

    private void OnDisable()
    {
        if (elementComponent != null)
            elementComponent.OnElementChanged -= UpdateVFX;
    }

    private void UpdateVFX(ElementType element)
    {
        if (fireVFX != null)
        {
            fireVFX.SetActive(element == ElementType.Fire);
        }

        if (waterVFX != null)
            waterVFX.SetActive(element == ElementType.Water);

        if (electricityVFX != null)
            electricityVFX.SetActive(element == ElementType.Electricity);
    }
}