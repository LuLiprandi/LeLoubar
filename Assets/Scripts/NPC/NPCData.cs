using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "LeLouBarr/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Sprite sprite;

    [Range(0.5f, 2f)]
    [Tooltip("Multiplicateur de patience pour varier la difficulté par personnage.")]
    public float patienceMultiplier = 1f;
}
