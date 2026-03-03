using UnityEngine;

public class NPCSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    /// <summary>Marks this slot as occupied by the given NPC.</summary>
    public void Assign()
    {
        IsOccupied = true;
    }

    /// <summary>Frees the slot so a new NPC can spawn here.</summary>
    public void Release()
    {
        IsOccupied = false;
    }
}
