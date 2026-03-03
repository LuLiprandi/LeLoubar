using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;

    [Header("Slots")]
    [SerializeField] private NPCSlot[] slots;

    [Header("NPC Pool")]
    [SerializeField] private NPCData[] npcDataPool;

    [Header("Spawn Timing")]
    [SerializeField] private float gameDuration = 600f;
    [SerializeField] private float spawnIntervalStart = 10f;
    [SerializeField] private float spawnIntervalEnd = 2f;
    [SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private readonly List<NPCData> _shuffledPool = new();
    private int _poolIndex;
    private float _elapsedTime;

    private void Start()
    {
        ShufflePool();

        // Spawn d'un seul NPC au lancement
        TrySpawn();

        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetCurrentInterval());
            TrySpawn();
        }
    }

    /// <summary>Returns the spawn interval based on elapsed time — decreases over the game duration.</summary>
    private float GetCurrentInterval()
    {
        float t = Mathf.Clamp01(_elapsedTime / gameDuration);
        return Mathf.Lerp(spawnIntervalStart, spawnIntervalEnd, difficultyCurve.Evaluate(t));
    }

    private void TrySpawn()
    {
        NPCSlot freeSlot = FindFreeSlot();
        if (freeSlot == null) return;

        SpawnAt(freeSlot, NextNPCData());
    }

    private void SpawnAt(NPCSlot slot, NPCData data)
    {
        GameObject npcGO = Instantiate(npcPrefab, slot.transform);
        npcGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        NPCController npc = npcGO.GetComponent<NPCController>();
        slot.Assign();
        npc.Initialize(data, slot);
    }

    private NPCSlot FindFreeSlot()
    {
        foreach (NPCSlot slot in slots)
            if (!slot.IsOccupied) return slot;
        return null;
    }

    private NPCData NextNPCData()
    {
        if (_poolIndex >= _shuffledPool.Count)
            ShufflePool();

        return _shuffledPool[_poolIndex++];
    }

    private void ShufflePool()
    {
        _shuffledPool.Clear();
        _shuffledPool.AddRange(npcDataPool);

        for (int i = _shuffledPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (_shuffledPool[i], _shuffledPool[j]) = (_shuffledPool[j], _shuffledPool[i]);
        }

        _poolIndex = 0;
    }
}
