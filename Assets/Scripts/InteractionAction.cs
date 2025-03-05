using UnityEngine;
using System;

[Serializable]
public class InteractionAction {
    public string actionName;         // Název akce, např. "Otevřít"
    public Sprite actionIcon;         // Ikona akce (můžeš ponechat prázdné, pokud ji nechceš používat)
    public Action actionCallback;     // Metoda, která se zavolá při výběru akce
    public string header;             // Header pro akci
    public string description;        // Popis akce (optional)
}