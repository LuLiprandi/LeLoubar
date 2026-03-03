using UnityEngine;

public class BeerItem : MonoBehaviour, IClickable, IHoverable
{
    public BeerType BeerType { get; private set; }
    public ToppingType Topping { get; private set; }
    public bool IsBeingDragged { get; set; }

    /// <summary>Initialise la bière avec son type.</summary>
    public void Initialize(BeerType beerType)
    {
        BeerType = beerType;
        Topping = ToppingType.None;
    }

    /// <summary>Ajoute un topping à la bière.</summary>
    public void AddTopping(ToppingType topping)
    {
        Topping = topping;
    }

    public void OnClick() { }

    public void OnHoverEnter() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnHoverExit() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
