using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;

    [Header("Slots")]
    [SerializeField] private NPCSlot[] slots;  // glisse les 3 slots ici

    [Header("NPC Pool")]
    [SerializeField] private NPCData[] npcDataPool;  // tous les NPCData SO

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 12f;

    // Pool mélangé pour éviter les répétitions
    private readonly List<NPCData> _shuffledPool = new();
    private int _poolIndex;

    private void Start()
    {
        ShufflePool();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        NPCSlot freeSlot = FindFreeSlot();
        if (freeSlot == null) return;  // tous les slots sont occupés

        NPCData data = NextNPCData();
        SpawnAt(freeSlot, data);
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

    // Fisher-Yates — garantit qu'on voit tous les personnages avant répétition
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

    private NPCData NextNPCData()
    {
        if (_poolIndex >= _shuffledPool.Count)
            ShufflePool();

        return _shuffledPool[_poolIndex++];
    }
}
