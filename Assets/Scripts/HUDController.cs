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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip punchClip;
    [SerializeField] private AudioClip gunClip;
    [SerializeField] private AudioClip slashClip;

    private int _currentHearts;
    private Coroutine _hitRoutine;
    private CanvasGroup _hitCanvasGroup;

    private void Awake()
    {
        _currentHearts = heartSlots.Length;
        RefreshHearts();

        if (hitFeedbackImage != null)
        {
            _hitCanvasGroup = hitFeedbackImage.gameObject.GetComponent<CanvasGroup>();
            if (_hitCanvasGroup == null)
                _hitCanvasGroup = hitFeedbackImage.gameObject.AddComponent<CanvasGroup>();

            _hitCanvasGroup.alpha = 0f;
            hitFeedbackImage.gameObject.SetActive(true);
        }
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
        int index = Random.Range(0, hitSprites.Length);
        hitFeedbackImage.sprite = hitSprites[index];
        _hitCanvasGroup.alpha = 1f;

        PlayHitSound(index);

        yield return new WaitForSeconds(hitDisplayDuration);

        _hitCanvasGroup.alpha = 0f;
        _hitRoutine = null;
    }

    private void PlayHitSound(int spriteIndex)
    {
        if (audioSource == null) return;

        AudioClip clip = spriteIndex switch
        {
            0 => punchClip,
            1 => gunClip,
            2 => slashClip,
            _ => punchClip
        };

        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
