using UnityEngine;

public class EntranceData : MonoBehaviour
{
    public static EntranceData Instance { get; private set; }

    public string lastEntranceKey = string.Empty;

    // Flag určující, zda se má ihned po načtení nové scény
    // vypsat debug log (používáme pro skok z Mars → Hab)
    public bool logAfterLoad = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}