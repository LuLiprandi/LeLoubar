using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image characterImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image patienceFill;

    [Header("Order")]
    [SerializeField] private OrderBubble orderBubble;

    [Header("Slide Animation")]
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Patience Colors")]
    [SerializeField] private Color colorHigh = Color.green;
    [SerializeField] private Color colorMid = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color colorLow = Color.red;

    public NPCOrder Order { get; private set; }

    public event Action OnHalfPatience;
    public event Action OnNPCLeft;

    private RectTransform _rectTransform;
    private NPCSlot _assignedSlot;
    private float _maxPatience;
    private float _currentPatience;
    private bool _isActive;
    private bool _halfTriggered;
    private bool _isDismissed;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>Initialise le NPC avec ses données et sa durée de patience.</summary>
    public void Initialize(NPCData data, NPCSlot slot, float patienceDuration)
    {
        _assignedSlot = slot;
        characterImage.sprite = data.sprite;
        _maxPatience = patienceDuration;
        _currentPatience = patienceDuration;
        _isActive = false;
        _halfTriggered = false;
        _isDismissed = false;

        Order = NPCOrder.GenerateRandom();
        orderBubble?.Setup(Order);

        UpdateGauge(1f);
        StartCoroutine(SlideRoutine(isIn: true));
    }

    private void Update()
    {
        if (!_isActive) return;

        _currentPatience -= Time.deltaTime;
        float ratio = Mathf.Clamp01(_currentPatience / _maxPatience);

        UpdateGauge(ratio);

        if (!_halfTriggered && ratio <= 0.5f)
        {
            _halfTriggered = true;
            OnHalfPatience?.Invoke();
        }

        if (_currentPatience <= 0f)
        {
            _isActive = false;
            TriggerAngry();
        }
    }

    private void UpdateGauge(float ratio)
    {
        if (patienceFill == null) return;

        patienceFill.fillAmount = ratio;

        patienceFill.color = ratio > 0.5f
            ? Color.Lerp(colorMid, colorHigh, (ratio - 0.5f) * 2f)
            : Color.Lerp(colorLow, colorMid, ratio * 2f);

        if (ratio < 0.25f)
        {
            float pulse = (Mathf.Sin(Time.time * 8f) + 1f) * 0.5f;
            patienceFill.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.15f, pulse);
        }
        else
        {
            patienceFill.transform.localScale = Vector3.one;
        }
    }

    private void TriggerAngry()
    {
        if (_isDismissed) return;
        Debug.Log($"[NPC] {gameObject.name} — patience épuisée.");
        OnNPCLeft?.Invoke();
        Dismiss();
    }

    /// <summary>Appelé quand la commande est correctement servie.</summary>
    public void Serve()
    {
        if (_isDismissed) return;
        _isActive = false;
        _isDismissed = true;
        Debug.Log($"[NPC] {gameObject.name} — servi avec succès.");
        OnNPCLeft?.Invoke();
        Dismiss();
    }

    /// <summary>Tente de servir le NPC avec la bière donnée. Retourne true si correct.</summary>
    public bool TryServe(BeerItem beer)
    {
        Debug.Log($"[TryServe] dismissed:{_isDismissed} | bière:{beer?.BeerType}/{beer?.Topping} | commande:{Order?.BeerType}/{Order?.Topping}");

        if (_isDismissed || beer == null || Order == null) return false;

        if (beer.BeerType == Order.BeerType && beer.Topping == Order.Topping)
        {
            Destroy(beer.gameObject);
            Serve();
            return true;
        }

        return false;
    }

    private void Dismiss()
    {
        _isDismissed = true;
        _isActive = false;
        StartCoroutine(SlideRoutine(isIn: false));
    }

    private IEnumerator SlideRoutine(bool isIn)
    {
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

        if (isIn)
            _isActive = true;
        else
        {
            _assignedSlot.Release();
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnPointerExit(PointerEventData eventData) =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);

    public void OnPointerClick(PointerEventData eventData) { }
}

