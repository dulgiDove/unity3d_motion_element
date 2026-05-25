public static class ElementReactionResolver
{
    public static ElementReactionType Resolve(ElementType current, ElementType incoming)
    {
        if (current == ElementType.None || incoming == ElementType.None)
            return ElementReactionType.None;

        if (current == incoming)
            return ElementReactionType.None;

        if (IsPair(current, incoming, ElementType.Fire, ElementType.Water))
            return ElementReactionType.Extinguish;

        if (IsPair(current, incoming, ElementType.Fire, ElementType.Electricity))
            return ElementReactionType.Overload;

        if (IsPair(current, incoming, ElementType.Water, ElementType.Electricity))
            return ElementReactionType.ElectroCharged;

        return ElementReactionType.None;
    }

    private static bool IsPair(ElementType a, ElementType b, ElementType x, ElementType y)
    {
        return (a == x && b == y) || (a == y && b == x);
    }
}