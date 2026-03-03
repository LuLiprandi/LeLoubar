using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image characterImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Slide Animation")]
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private float slideDuration = 0.5f;

    private RectTransform _rectTransform;
    private NPCSlot _assignedSlot;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>Sets up the NPC visuals and triggers the slide-in animation.</summary>
    public void Initialize(NPCData data, NPCSlot slot)
    {
        _assignedSlot = slot;
        characterImage.sprite = data.sprite;
        StartCoroutine(SlideRoutine(isIn: true));
    }

    /// <summary>Triggers the slide-out animation then destroys the GameObject.</summary>
    public void Dismiss()
    {
        StartCoroutine(SlideRoutine(isIn: false));
    }

    private IEnumerator SlideRoutine(bool isIn)
    {
        // Slide-in  : hidden position → slot position (0,0)
        // Slide-out : slot position (0,0) → hidden position
        Vector2 slotPos = Vector2.zero;
        Vector2 hiddenPos = slotPos - new Vector2(0f, slideDistance);

        Vector2 startPos = isIn ? hiddenPos : slotPos;
        Vector2 endPos = isIn ? slotPos : hiddenPos;
        float startAlpha = isIn ? 0f : 1f;
        float endAlpha = isIn ? 1f : 0f;

        _rectTransform.anchoredPosition = startPos;
        canvasGroup.alpha = startAlpha;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);
            _rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, t);
            canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, t);
            yield return null;
        }

        _rectTransform.anchoredPosition = endPos;
        canvasGroup.alpha = endAlpha;

        if (!isIn)
        {
            _assignedSlot.Release();
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[NPC] Clicked: {gameObject.name}");
    }
}
