using System.Collections;
using UnityEngine;

public class BeerTap : MonoBehaviour, IClickable, IHoverable
{
    private const float BrewDurationSeconds = 2f;

    [Header("Config")]
    [SerializeField] private BeerType beerType;
    [SerializeField] private GameObject beerPrefab;
    [SerializeField] private Transform beerSpawnPoint;

    [Header("Lever Animation")]
    [SerializeField] private Transform leverTransform;
    [SerializeField] private float leverAngle = 30f;
    [SerializeField] private float leverSpeed = 5f;

    public bool IsProducing { get; private set; }

    /// <summary>Déclenché au clic — lance la production si disponible.</summary>
    public void OnClick()
    {
        if (IsProducing)
        {
            Debug.Log("[BeerTap] Déjà en production.");
            return;
        }

        StartCoroutine(BrewRoutine());
    }

    private IEnumerator BrewRoutine()
    {
        IsProducing = true;

        // Animation levier — descend
        yield return StartCoroutine(AnimateLever(leverAngle));

        yield return new WaitForSeconds(BrewDurationSeconds);

        // Animation levier — remonte
        yield return StartCoroutine(AnimateLever(0f));

        SpawnBeer();
        IsProducing = false;
    }

    private IEnumerator AnimateLever(float targetAngle)
    {
        if (leverTransform == null) yield break;

        Quaternion startRot = leverTransform.localRotation;
        Quaternion targetRot = Quaternion.Euler(targetAngle, 0f, 0f);
        float elapsed = 0f;
        float duration = 1f / leverSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
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

        GameObject beerGO = Instantiate(beerPrefab, beerSpawnPoint.position, beerSpawnPoint.rotation);

        if (beerGO.TryGetComponent(out BeerItem beerItem))
            beerItem.Initialize(beerType);
    }

    public void OnHoverEnter() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Hover);

    public void OnHoverExit() =>
        CursorController.Instance?.SetState(CursorController.CursorState.Default);
}
