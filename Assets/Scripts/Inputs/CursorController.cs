using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance { get; private set; }

    public enum CursorState { Default, Hover, Drag }

    [Header("Cursor Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite dragSprite;

    [Header("References")]
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private Image cursorImage;

    private CursorState _currentState = CursorState.Default;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Hide the OS cursor — our UI image replaces it
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        MoveCursorToMousePosition();
    }

    private void MoveCursorToMousePosition()
    {
        // Reuse the same Vector2 read — no allocation
        cursorRect.position = Mouse.current.position.ReadValue();
    }

    /// <summary>Sets the cursor visual state.</summary>
    public void SetState(CursorState state)
    {
        if (_currentState == state) return;

        _currentState = state;
        cursorImage.sprite = state switch
        {
            CursorState.Hover => hoverSprite,
            CursorState.Drag => dragSprite,
            _ => defaultSprite
        };
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
