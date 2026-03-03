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

    [Header("Patience Duration")]
    [SerializeField] private float initialPatienceDuration = 20f;
    [SerializeField] private float minPatienceDuration = 5f;
    [SerializeField] private float patienceReductionPerNPC = 0.5f;

    private readonly List<NPCData> _shuffledPool = new();
    private int _poolIndex;
    private float _currentPatienceDuration;

    private void Start()
    {
        _currentPatienceDuration = initialPatienceDuration;
        ShufflePool();
        TrySpawn();
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
        npc.Initialize(data, slot, _currentPatienceDuration);

        // Le spawn du suivant est lié à la jauge à 50%
        npc.OnHalfPatience += TrySpawn;
        npc.OnNPCLeft += TrySpawn;

        // Réduit la durée pour le prochain NPC
        _currentPatienceDuration = Mathf.Max(
            minPatienceDuration,
            _currentPatienceDuration - patienceReductionPerNPC
        );
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
