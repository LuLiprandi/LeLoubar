using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private const float DropMargin = 150f;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    /// <summary>Retourne true si la position écran est dans la zone de la poubelle.</summary>
    public bool IsUnderMouse(Vector2 mousePos)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return false;

        Vector3 screenCenter = mainCamera.WorldToScreenPoint(col.bounds.center);
        return Vector2.Distance(mousePos, screenCenter) < DropMargin;
    }
}
