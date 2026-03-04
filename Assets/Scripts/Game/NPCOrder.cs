using UnityEngine;

/// <summary>Stores a single NPC order: beer type and optional topping.</summary>
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

    /// <summary>
    /// Generates a random order based on game progression.
    /// gameProgress = 0 (start) → almost no toppings.
    /// gameProgress = 1 (end)   → high chance of cigarette or pill.
    /// </summary>
    public static NPCOrder GenerateRandom(float gameProgress = 0f)
    {
        BeerType beer = (BeerType)Random.Range(0, 3);

        // Topping probability scales from 5% at the start to 80% at the end.
        float toppingChance = Mathf.Lerp(0.05f, 0.80f, gameProgress);
        bool hasTopping = Random.value < toppingChance;

        ToppingType topping = hasTopping
            ? (ToppingType)Random.Range(1, 3) // 1=Cigarette, 2=Pill
            : ToppingType.None;

        return new NPCOrder(beer, topping);
    }

    /// <summary>Returns true if the given order matches this one exactly.</summary>
    public bool Matches(NPCOrder other)
    {
        return BeerType == other.BeerType && Topping == other.Topping;
    }
}

