using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PinPadSceneTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pinCanvas;
    public TMP_Text displayText;
    public Button[] numberButtons;
    public Button submitButton;
    public Button clearButton;

    [Header("PIN Settings")]
    public string correctCode = "4832";

    [Header("Scene Settings")]
    public string sceneToLoad = "EndingCutscene";

    private string input = string.Empty;
    private bool playerInRange = false;
    private Inventory inventoryManager;

    void Awake()
    {
        if (pinCanvas != null)
            pinCanvas.SetActive(false);

        // Najde InventoryManager ve scéně
        inventoryManager = FindObjectOfType<Inventory>();
        if (inventoryManager == null)
        {
            Debug.LogWarning("PinPadSceneTrigger: InventoryManager nenalezen.");
        }
    }

    void Start()
    {
        foreach (var btn in numberButtons)
        {
            string num = btn.GetComponentInChildren<TMP_Text>().text;
            btn.onClick.AddListener(() => OnNumberPressed(num));
        }
        submitButton.onClick.AddListener(OnSubmit);
        clearButton.onClick.AddListener(OnClear);
    }

    void Update()
    {
        if (!playerInRange)
            return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool open = !pinCanvas.activeSelf;
            pinCanvas.SetActive(open);
            if (open) OnClear();
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
            playerInRange = false;
    }

    private void OnNumberPressed(string num)
    {
        if (input.Length >= correctCode.Length) return;

        input += num;
        UpdateDisplay();
    }

    private void OnClear()
    {
        input = string.Empty;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        displayText.text = new string('*', input.Length);
    }

    private void OnSubmit()
    {
        if (input == correctCode)
        {
            pinCanvas.SetActive(false);

            if (inventoryManager != null)
            {
                inventoryManager.gameObject.SetActive(false);
                Debug.Log("InventoryManager byl vypnut.");
            }

            var fader = FindObjectOfType<SceneFader>();
            if (fader != null)
            {
                fader.FadeToScene(sceneToLoad);
            }
            else
            {
                Debug.LogWarning("PinPadSceneTrigger: SceneFader nenalezen, načítám přímo.");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            StartCoroutine(ShowError());
        }
    }

    private System.Collections.IEnumerator ShowError()
    {
        var originalColor = displayText.color;
        displayText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        displayText.color = originalColor;
        OnClear();
    }
}
