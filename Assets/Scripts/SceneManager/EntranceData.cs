using UnityEngine;

public class EntranceData : MonoBehaviour
{
    public static EntranceData Instance { get; private set; }

    public string lastEntranceKey = string.Empty;
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