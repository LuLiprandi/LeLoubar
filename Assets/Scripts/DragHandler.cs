using UnityEngine;
using UnityEngine.InputSystem;

public class DragHandler : MonoBehaviour
{
    public static DragHandler Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragHeight = 2f;
    [SerializeField] private float dropMargin = 300f;

    public BeerItem DraggedBeer { get; private set; }
    public bool IsDragging => DraggedBeer != null;

    private Vector3 _originPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!IsDragging) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        Plane plane = new Plane(Vector3.up, Vector3.up * dragHeight);
        if (plane.Raycast(ray, out float distance))
            DraggedBeer.transform.position = ray.GetPoint(distance);

        if (!Mouse.current.leftButton.isPressed)
            Drop();
    }

    /// <summary>Démarre le drag — mémorise la position d'origine.</summary>
    public void StartDrag(BeerItem beer)
    {
        DraggedBeer = beer;
        _originPosition = beer.transform.position;
        CursorController.Instance?.SetState(CursorController.CursorState.Drag);
    }

    private void Drop()
    {
        if (DraggedBeer == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        NPCController closestNPC = FindNPCUnderMouse(mousePos);

        if (closestNPC != null)
        {
            bool served = closestNPC.TryServe(DraggedBeer);
            if (!served && DraggedBeer != null)
                DraggedBeer.transform.position = _originPosition;
        }
        else
        {
            DraggedBeer.transform.position = _originPosition;
        }

        CursorController.Instance?.SetState(CursorController.CursorState.Default);
        DraggedBeer = null;
    }

    private NPCController FindNPCUnderMouse(Vector2 mousePos)
    {
        NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);

        foreach (NPCController npc in npcs)
        {
            RectTransform rect = npc.GetComponent<RectTransform>();
            if (rect == null) continue;

            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            Vector2 min = RectTransformUtility.WorldToScreenPoint(mainCamera, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(mainCamera, corners[2]);

            min -= Vector2.one * dropMargin;
            max += Vector2.one * dropMargin;

            if (mousePos.x >= min.x && mousePos.x <= max.x &&
                mousePos.y >= min.y && mousePos.y <= max.y)
                return npc;
        }

        return null;
    }
}
