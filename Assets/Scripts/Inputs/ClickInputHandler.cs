using UnityEngine;
using UnityEngine.InputSystem;

public class ClickInputHandler : MonoBehaviour
{
    public static ClickInputHandler Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private float maxRayDistance = 100f;

    private IHoverable _currentHovered;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        // Bloque le raycast pendant le drag
        if (DragHandler.Instance != null && DragHandler.Instance.IsDragging) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance, clickableLayers);

        HandleHover(hit, hitInfo);

        if (hit && Mouse.current.leftButton.wasPressedThisFrame)
            HandleClick(hitInfo);
    }

    private void HandleHover(bool hit, RaycastHit hitInfo)
    {
        IHoverable hoverable = hit
            ? hitInfo.collider.GetComponent<IHoverable>()
            : null;

        if (hoverable == _currentHovered) return;

        _currentHovered?.OnHoverExit();
        _currentHovered = hoverable;

        if (_currentHovered != null)
        {
            _currentHovered.OnHoverEnter();
            CursorController.Instance?.SetState(CursorController.CursorState.Hover);
        }
        else
        {
            CursorController.Instance?.SetState(CursorController.CursorState.Default);
        }
    }

    private void HandleClick(RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out IClickable clickable))
            clickable.OnClick();
    }

    /// <summary>Call this when a drag starts to update cursor state.</summary>
    public void NotifyDragStart() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Drag);

    /// <summary>Call this when a drag ends to restore cursor state.</summary>
    public void NotifyDragEnd() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
