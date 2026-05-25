using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ElementReceiver : MonoBehaviour
{
    [SerializeField] private ElementComponent elementComponent;
    [SerializeField] private FlammableProperty flammableProperty;
    [SerializeField] private WettableProperty wettableProperty;
    [SerializeField] private ConductiveProperty conductiveProperty;
    [SerializeField] private PermanentElementProperty permanentElementProperty;

    private void Awake()
    {
        if (elementComponent == null)
            elementComponent = GetComponentInParent<ElementComponent>();

        if (flammableProperty == null)
            flammableProperty = GetComponentInParent<FlammableProperty>();

        if (wettableProperty == null)
            wettableProperty = GetComponentInParent<WettableProperty>();

        if (conductiveProperty == null)
            conductiveProperty = GetComponentInParent<ConductiveProperty>();

        if (permanentElementProperty == null)
            permanentElementProperty = GetComponentInParent<PermanentElementProperty>();


        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryReceiveElement(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryReceiveElement(other);
    }

    private void TryReceiveElement(Collider other)
    {
        ElementSource source = other.GetComponentInParent<ElementSource>();

        if (source == null || !source.HasElement)
            return;

        Vector3 reactionPosition = other.ClosestPoint(transform.position);

        ReceiveElement(source.Element, reactionPosition);
    }

    public void ReceiveElement(ElementType incomingElement, Vector3 reactionPosition)
    {
        if (elementComponent == null)
            return;

        if (incomingElement == ElementType.None)
            return;

        ElementType currentElement = elementComponent.CurrentElement;

        ElementReactionType reaction =
            ElementReactionResolver.Resolve(currentElement, incomingElement);

        if (reaction != ElementReactionType.None)
        {
            HandleReaction(reaction, incomingElement, reactionPosition);
            return;
        }

        if (currentElement != ElementType.None)
            return;

        switch (incomingElement)
        {
            case ElementType.Fire:
                ReceiveFire();
                break;

            case ElementType.Water:
                ReceiveWater();
                break;

            case ElementType.Electricity:
                ReceiveElectricity();
                break;
        }
    }

    private void ReceiveFire()
    {
        if (wettableProperty != null && wettableProperty.IsWet)
        {
            wettableProperty.Dry();
            return;
        }

        if (flammableProperty != null)
        {
            flammableProperty.Ignite();
        }
    }

    private void ReceiveWater()
    {
        if (flammableProperty != null && flammableProperty.IsBurning)
        {
            flammableProperty.Extinguish();
        }

        if (wettableProperty != null)
        {
            wettableProperty.ApplyWater();
        }
    }

    private void ReceiveElectricity()
    {
        bool canConduct =
            conductiveProperty != null ||
            elementComponent.CurrentElement == ElementType.Water;

        if (!canConduct)
        {
            return;
        }

        if (conductiveProperty != null)
            conductiveProperty.ApplyElectricity();
        else
            elementComponent.SetElement(ElementType.Electricity);
    }

    private void HandleReaction(
        ElementReactionType reaction,
        ElementType incomingElement,
        Vector3 reactionPosition
    )
    {
        switch (reaction)
        {
            case ElementReactionType.Extinguish:
                if (elementComponent.CurrentElement == ElementType.Fire)
                {
                    elementComponent.ClearElement();

                    if (wettableProperty != null)
                        wettableProperty.ApplyWater();

                    if (ElementReactionManager.Instance != null)
                        ElementReactionManager.Instance.ExecuteReaction(
                            reaction,
                            transform.position
                        );
                }

                break;

            case ElementReactionType.Overload:
                if (ElementReactionManager.Instance != null)
                    ElementReactionManager.Instance.ExecuteReaction(
                        reaction,
                        reactionPosition
                    );

                break;

            case ElementReactionType.ElectroCharged:

                if (elementComponent.CurrentElement == ElementType.Water)
                {
                    if (ElementReactionManager.Instance != null)
                    {
                        ElementReactionManager.Instance.ExecuteReaction(
                            reaction,
                            reactionPosition,
                            elementComponent,
                            GetComponent<Collider>()
                        );
                    }
                }

                break;
        }
    }
}