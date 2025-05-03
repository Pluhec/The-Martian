using UnityEngine;

/// <summary>
/// Označuje v dané scéně bod pro šipku k questu.
/// Každý objekt, na který má šipka směřovat, dostaň tuto komponentu
/// a vyplň questID + pořadí (targetIndex).
/// </summary>
public class QuestTarget : MonoBehaviour
{
    [Tooltip("ID questu, musí souhlasit s Quest.questID")]
    public int questID;
    [Tooltip("Index podcíle, od 0 do ...")]
    public int targetIndex;
}