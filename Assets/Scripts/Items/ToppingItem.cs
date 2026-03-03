using UnityEngine;

public class ToppingItem : MonoBehaviour, IClickable, IHoverable
{
    [Header("Config")]
    [SerializeField] private ToppingType toppingType;

    /// <summary>Démarre le drag du topping.</summary>
    public void OnClick()
    {
        if (DragHandler.Instance == null) return;
        DragHandler.Instance.StartToppingDrag(this);
    }

    public ToppingType GetToppingType() => toppingType;

    public void OnHoverEnter() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnHoverExit() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
