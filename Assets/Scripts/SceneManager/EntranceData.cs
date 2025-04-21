using UnityEngine;

public class EntranceData : MonoBehaviour
{
    public static EntranceData Instance { get; private set; }
    public string lastEntranceKey;

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