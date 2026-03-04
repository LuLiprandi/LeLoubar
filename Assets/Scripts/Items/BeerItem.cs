using UnityEngine;

public class BeerItem : MonoBehaviour, IClickable, IHoverable
{
    public BeerType BeerType { get; private set; }
    public ToppingType Topping { get; private set; }

    [Header("Topping Meshes")]
    [SerializeField] private GameObject cigaretteMeshRoot;
    [SerializeField] private GameObject pillMeshRoot;

    /// <summary>Initialise la bière avec son type.</summary>
    public void Initialize(BeerType beerType)
    {
        BeerType = beerType;
        Topping = ToppingType.None;

        if (cigaretteMeshRoot != null) cigaretteMeshRoot.SetActive(false);
        if (pillMeshRoot != null) pillMeshRoot.SetActive(false);
    }

    /// <summary>Ajoute un topping à la bière et swape le mesh.</summary>
    public void AddTopping(ToppingType topping)
    {
        if (Topping != ToppingType.None) return; // déjà un topping

        Topping = topping;

        switch (topping)
        {
            case ToppingType.Cigarette:
                if (cigaretteMeshRoot != null) cigaretteMeshRoot.SetActive(true);
                break;
            case ToppingType.Pill:
                if (pillMeshRoot != null) pillMeshRoot.SetActive(true);
                break;
        }
    }

    public void OnClick()
    {
        if (DragHandler.Instance == null) return;
        DragHandler.Instance.StartDrag(this);
    }

    public void OnHoverEnter() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnHoverExit() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
