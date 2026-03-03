using UnityEngine;

[CreateAssetMenu(fileName = "BeerConfig", menuName = "LeLouBarr/Beer Config")]
public class BeerConfig : ScriptableObject
{
    [Header("Blonde")]
    public Sprite blondePlain;
    public Sprite blondeCigarette;
    public Sprite blondePill;

    [Header("Ambrée")]
    public Sprite amberPlain;
    public Sprite amberCigarette;
    public Sprite amberPill;

    [Header("Rouge")]
    public Sprite redPlain;
    public Sprite redCigarette;
    public Sprite redPill;

    /// <summary>Returns the correct sprite for the given beer and topping combination.</summary>
    public Sprite GetSprite(BeerType beer, ToppingType topping)
    {
        return beer switch
        {
            BeerType.Blonde => topping switch
            {
                ToppingType.Cigarette => blondeCigarette,
                ToppingType.Pill => blondePill,
                _ => blondePlain
            },
            BeerType.Amber => topping switch
            {
                ToppingType.Cigarette => amberCigarette,
                ToppingType.Pill => amberPill,
                _ => amberPlain
            },
            BeerType.Red => topping switch
            {
                ToppingType.Cigarette => redCigarette,
                ToppingType.Pill => redPill,
                _ => redPlain
            },
            _ => null
        };
    }
}
