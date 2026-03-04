using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] private Image[] heartSlots;

    [Header("Hit Feedback")]
    [SerializeField] private Image hitFeedbackImage;
    [SerializeField] private Sprite[] hitSprites;
    [SerializeField] private float hitDisplayDuration = 1.5f;

    private int _currentHearts;
    private Coroutine _hitRoutine;

    private void Awake()
    {
        _currentHearts = heartSlots.Length;
        RefreshHearts();

        if (hitFeedbackImage != null)
            hitFeedbackImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnHeartsChanged += HandleHeartsChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnHeartsChanged -= HandleHeartsChanged;
    }

    private void HandleHeartsChanged(int hearts)
    {
        _currentHearts = hearts;
        RefreshHearts();
        ShowHitFeedback();
    }

    private void RefreshHearts()
    {
        for (int i = 0; i < heartSlots.Length; i++)
            heartSlots[i].gameObject.SetActive(i < _currentHearts);
    }

    private void ShowHitFeedback()
    {
        if (hitFeedbackImage == null || hitSprites == null || hitSprites.Length == 0) return;

        if (_hitRoutine != null) StopCoroutine(_hitRoutine);
        _hitRoutine = StartCoroutine(HitFeedbackRoutine());
    }

    private IEnumerator HitFeedbackRoutine()
    {
        hitFeedbackImage.sprite = hitSprites[Random.Range(0, hitSprites.Length)];
        hitFeedbackImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(hitDisplayDuration);

        hitFeedbackImage.gameObject.SetActive(false);
        _hitRoutine = null;
    }
}
