using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Ovládá PIN panel a přepne scénu pomocí SceneFader.FadeToScene().
/// Script automaticky najde instanci SceneFader ve scéně.
/// </summary>
public class PinPadSceneTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pinCanvas;
    public TMP_Text displayText;
    public Button[] numberButtons;
    public Button submitButton;
    public Button clearButton;

    [Header("PIN Settings")]
    public string correctCode = "1234";

    [Header("Scene Settings")]
    public string sceneToLoad = "EndingCutscene";

    private string input = string.Empty;
    private bool playerInRange = false;

    void Awake()
    {
        if (pinCanvas != null)
            pinCanvas.SetActive(false);
    }

    void Start()
    {
        // Nastavení listenerů na číselná tlačítka
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

        // Otevření/zavření panelu klávesou E
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool open = !pinCanvas.activeSelf;
            pinCanvas.SetActive(open);
            if (open) OnClear();
        }

        // Zavření ESC
        if (pinCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            pinCanvas.SetActive(false);
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
        displayText.text = input;
    }

    private void OnSubmit()
    {
        if (input == correctCode)
        {
            pinCanvas.SetActive(false);
            // Najdi SceneFader ve scéně a přepni scénu fade efektem
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
