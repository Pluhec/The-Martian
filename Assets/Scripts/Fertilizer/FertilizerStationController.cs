using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FertilizerStationController : MonoBehaviour
{
    public static FertilizerStationController Instance { get; private set; }

    [Header("UI Root (Canvas), start inactive)")]
    public GameObject uiRoot;

    [Header("Reference na sloty a tlačítko v UI")]
    public InputSlotHandler inputSlot;
    public WorkAreaDropHandler workArea;
    public Slot outputSlot;
    public Button unwrapButton;

    [Header("ItemDefinition IDs")]
    public string shitPackID;
    public GameObject compostPrefab;

    [Header("Animation")]
    public Image unwrapAnimImage;
    public Sprite[] unwrapFrames;
    public float frameRate = 12f;

    [Header("Audio")]
    public AudioManager audioManager;
    
    
    [HideInInspector] public bool hasPackInInput;
    [HideInInspector] public bool hasPackOnWorkArea;
    bool isAnimating;
    
    bool playerInRange = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        uiRoot.SetActive(false);
        unwrapButton.interactable = false;
        unwrapAnimImage.gameObject.SetActive(false);

        inputSlot.station = this;
        workArea.station  = this;
        unwrapButton.onClick.AddListener(OnClickUnwrap);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        unwrapButton.onClick.RemoveListener(OnClickUnwrap);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            bool open = !uiRoot.activeSelf;
            uiRoot.SetActive(open);
            ItemButton.isDragEnabled = open;
            if (!open)
                ResetStation();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (uiRoot.activeSelf)
            {
                uiRoot.SetActive(false);
                ItemButton.isDragEnabled = false;
                ResetStation();
            }
        }
    }

    public void OnShitPackReceived(ItemButton btn)
    {
        hasPackInInput = true;
    }

    public void OnWorkAreaDrop(GameObject go)
    {
        if (hasPackOnWorkArea || isAnimating) return;

        hasPackOnWorkArea       = true;
        unwrapButton.interactable = true;

        unwrapAnimImage.sprite = unwrapFrames[0];
        unwrapAnimImage.gameObject.SetActive(true);
    }

    void OnClickUnwrap()
    {
        if (!hasPackOnWorkArea || isAnimating) return;
        StartCoroutine(PlayUnwrapAnimation());
    }

    IEnumerator PlayUnwrapAnimation()
    {
        isAnimating = true;
        unwrapButton.interactable = false;

        if (audioManager != null)
        {
            audioManager.PlayPoopPackOpenning();
            Debug.Log("prehravam zvuk");
        }
        else
        {
            Debug.Log("no audio manager");
        }
            

        float delay = 1f / frameRate;
        foreach (var frame in unwrapFrames)
        {
            unwrapAnimImage.sprite = frame;
            yield return new WaitForSeconds(delay);
        }

        unwrapAnimImage.gameObject.SetActive(false);

        if (hasPackInInput)
        {
            inputSlot.Clear();
            hasPackInInput = false;
        }

        var result = Instantiate(compostPrefab);
        outputSlot.OnDirectAdd(result, 1);

        hasPackOnWorkArea = false;
        isAnimating       = false;
    }

    void ResetStation()
    {
        hasPackOnWorkArea = isAnimating = false;
        unwrapButton.interactable = false;
        unwrapAnimImage.gameObject.SetActive(false);
    }
}