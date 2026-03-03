/// <summary>Stores a single NPC order — beer type and optional topping.</summary>
public class NPCOrder
{
    public BeerType BeerType { get; }
    public ToppingType Topping { get; }
    public bool HasTopping => Topping != ToppingType.None;

    public NPCOrder(BeerType beer, ToppingType topping)
    {
        BeerType = beer;
        Topping = topping;
    }

    /// <summary>Generates a random order.</summary>
    public static NPCOrder GenerateRandom()
    {
        BeerType beer = (BeerType)UnityEngine.Random.Range(0, 3);

        bool hasTopping = UnityEngine.Random.value > 0.5f;
        ToppingType topping = hasTopping
            ? (ToppingType)UnityEngine.Random.Range(1, 3) // 1=Cigarette, 2=Pill
            : ToppingType.None;

        return new NPCOrder(beer, topping);
    }

    /// <summary>Returns true if the given order matches this one exactly.</summary>
    public bool Matches(NPCOrder other)
    {
        return BeerType == other.BeerType && Topping == other.Topping;
    }
}
