using UnityEngine;
using UnityEngine.InputSystem;

public class DragHandler : MonoBehaviour
{
    public static DragHandler Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragHeight = 2f;
    [SerializeField] private float dropMargin = 50f;

    public BeerItem DraggedBeer { get; private set; }
    public ToppingItem DraggedTopping { get; private set; }
    public bool IsDragging => DraggedBeer != null || DraggedTopping != null;

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
        {
            Vector3 worldPos = ray.GetPoint(distance);

            if (DraggedBeer != null)
                DraggedBeer.transform.position = worldPos;
            else if (DraggedTopping != null)
                DraggedTopping.transform.position = worldPos;
        }

        if (!Mouse.current.leftButton.isPressed)
            Drop();
    }

    /// <summary>Démarre le drag d'une bière.</summary>
    public void StartDrag(BeerItem beer)
    {
        if (IsDragging) return;
        DraggedBeer = beer;
        _originPosition = beer.transform.position;
        CursorController.Instance?.SetState(CursorController.CursorState.Drag);
    }

    /// <summary>Démarre le drag d'un topping.</summary>
    public void StartToppingDrag(ToppingItem topping)
    {
        if (IsDragging) return;
        DraggedTopping = topping;
        _originPosition = topping.transform.position;
        CursorController.Instance?.SetState(CursorController.CursorState.Drag);
    }

    private void Drop()
    {
        if (!IsDragging) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (DraggedBeer != null)
            DropBeer(mousePos);
        else if (DraggedTopping != null)
            DropTopping(mousePos);

        CursorController.Instance?.SetState(CursorController.CursorState.Default);
        DraggedBeer = null;
        DraggedTopping = null;
    }

    private void DropBeer(Vector2 mousePos)
    {
        NPCController closestNPC = FindNPCUnderMouse(mousePos);

        if (closestNPC != null)
        {
            bool served = closestNPC.TryServe(DraggedBeer);
            if (!served && DraggedBeer != null)
                DraggedBeer.transform.position = _originPosition;
        }
        else
        {
            if (DraggedBeer != null)
                DraggedBeer.transform.position = _originPosition;
        }
    }

    private void DropTopping(Vector2 mousePos)
    {
        BeerItem targetBeer = FindBeerUnderMouse(mousePos);

        if (targetBeer != null)
        {
            targetBeer.AddTopping(DraggedTopping.GetToppingType());
            Debug.Log($"[Topping] {DraggedTopping.GetToppingType()} ajouté à {targetBeer.BeerType}");
        }

        // Le topping revient toujours à sa place — stock infini
        DraggedTopping.transform.position = _originPosition;
    }

    private NPCController FindNPCUnderMouse(Vector2 mousePos)
    {
        NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        NPCController closest = null;
        float closestDist = float.MaxValue;

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
            {
                Vector2 center = (min + max) / 2f;
                float dist = Vector2.Distance(mousePos, center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = npc;
                }
            }
        }

        return closest;
    }

    private BeerItem FindBeerUnderMouse(Vector2 mousePos)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.TryGetComponent(out BeerItem beer))
                return beer;

            if (hit.collider.GetComponentInParent<BeerItem>() is BeerItem parentBeer)
                return parentBeer;
        }

        return null;
    }
}
