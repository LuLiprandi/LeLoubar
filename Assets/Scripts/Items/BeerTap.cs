using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeerTap : MonoBehaviour, IClickable, IHoverable
{
    private const float BrewDurationSeconds = 4f;

    [Header("Config")]
    [SerializeField] private BeerType beerType;
    [SerializeField] private GameObject beerPrefab;
    [SerializeField] private Transform beerSpawnPoint;

    [Header("Spawn Rotation Override")]
    [SerializeField] private Vector3 spawnRotationEuler = new Vector3(90f, 0f, 0f);

    [Header("Lever Animation")]
    [SerializeField] private Transform leverTransform;
    [SerializeField] private float leverAngle = 30f;
    [SerializeField] private float leverSpeed = 3f;

    [Header("Brew Gauge")]
    [SerializeField] private Image gaugeImage;
    [SerializeField] private RectTransform gaugeRect;

    private Quaternion _initialRotation;
    public bool IsProducing { get; private set; }

    private void Start()
    {
        if (leverTransform != null)
            _initialRotation = leverTransform.localRotation;

        if (gaugeImage != null)
            gaugeImage.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (IsProducing) return;
        StartCoroutine(BrewRoutine());
    }

    private IEnumerator BrewRoutine()
    {
        IsProducing = true;

        if (gaugeImage != null)
        {
            gaugeImage.gameObject.SetActive(true);
            gaugeImage.fillAmount = 0f;
        }

        yield return StartCoroutine(AnimateLever(_initialRotation * Quaternion.Euler(leverAngle, 0f, 0f)));

        float elapsed = 0f;
        while (elapsed < BrewDurationSeconds)
        {
            elapsed += Time.deltaTime;

            if (gaugeImage != null)
            {
                gaugeImage.fillAmount = elapsed / BrewDurationSeconds;

                if (gaugeRect != null)
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
                    gaugeRect.position = screenPos;
                }
            }

            yield return null;
        }

        if (gaugeImage != null)
            gaugeImage.gameObject.SetActive(false);

        yield return StartCoroutine(AnimateLever(_initialRotation));

        SpawnBeer();
        IsProducing = false;
    }

    private IEnumerator AnimateLever(Quaternion targetRot)
    {
        if (leverTransform == null) yield break;

        Quaternion startRot = leverTransform.localRotation;
        float elapsed = 0f;
        float duration = 1f / leverSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            leverTransform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        leverTransform.localRotation = targetRot;
    }

    private void SpawnBeer()
    {
        if (beerPrefab == null || beerSpawnPoint == null)
        {
            Debug.LogWarning("[BeerTap] Prefab ou SpawnPoint manquant.");
            return;
        }

        Quaternion spawnRotation = Quaternion.Euler(spawnRotationEuler);
        GameObject beerGO = Instantiate(beerPrefab, beerSpawnPoint.position, spawnRotation);

        if (beerGO.TryGetComponent(out BeerItem beerItem))
            beerItem.Initialize(beerType);
    }

    public void OnHoverEnter() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnHoverExit() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
