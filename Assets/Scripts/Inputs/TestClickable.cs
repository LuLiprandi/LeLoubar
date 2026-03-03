using UnityEngine;

public class TestClickable : MonoBehaviour, IClickable, IHoverable
{
    public void OnClick() => Debug.Log($"[Click] {gameObject.name}");
    public void OnHoverEnter() => Debug.Log($"[Hover Enter] {gameObject.name}");
    public void OnHoverExit() => Debug.Log($"[Hover Exit] {gameObject.name}");
}
