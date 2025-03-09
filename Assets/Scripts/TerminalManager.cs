using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    public float typewriterSpeed = 80f;
    private float charDelay { get { return 1f / typewriterSpeed; } }

    private List<string> commandHistory = new List<string>(); 
    private int historyIndex = -1; 

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
        terminalInput.ActivateInputField();
        terminalInput.Select();
    
        List<string> asciiLogo = interpreter.LoadAsciiLogo();
        StartCoroutine(ProcessInterpreterLines(asciiLogo, false));
    }

    private void Update()
    {
        HandleCommandHistory();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            string userInput = terminalInput.text;
            ClearInputField();
            
            AddEmptyLine();
            
            if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != userInput)
            {
                commandHistory.Add(userInput);
            }
            historyIndex = commandHistory.Count;

            AddDirectoryLine(userInput);

            List<string> interpretation = interpreter.Interpret(userInput);
            
            if (interpretation.Count > 0 && interpretation[0] == "CLEAR_TERMINAL")
            {
                ClearTerminal();
            }
            else
            {
                StartCoroutine(ProcessInterpreterLines(interpretation));
            }

            userInputLine.transform.SetAsLastSibling();
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    void HandleCommandHistory()
    {
        if (commandHistory.Count == 0) return;

        if (terminalInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    terminalInput.text = commandHistory[historyIndex];
                    terminalInput.caretPosition = terminalInput.text.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    terminalInput.text = commandHistory[historyIndex];
                }
                else
                {
                    historyIndex = commandHistory.Count;
                    terminalInput.text = "";
                }
                terminalInput.caretPosition = terminalInput.text.Length;
            }
        }
    }

    void ClearInputField()
    {
        terminalInput.text = "";
    }

    void AddDirectoryLine(string userInput)
    {
        RectTransform msgListRT = msgList.GetComponent<RectTransform>();
        msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

        GameObject msg = Instantiate(directoryLine, msgList.transform);
        msg.transform.SetSiblingIndex(userInputLine.transform.GetSiblingIndex());

        TextMeshProUGUI[] texts = msg.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 1)
        {
            texts[1].text = userInput;
        }
        else
        {
            Debug.LogError("Expected at least 2 TextMeshProUGUI components in directoryLine prefab.");
        }
    }

    IEnumerator ProcessInterpreterLines(List<string> interpretation, bool useTypewriterEffect = true)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            RectTransform msgListRT = msgList.GetComponent<RectTransform>();
            msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

            GameObject res = Instantiate(responseLine, msgList.transform);
            res.transform.SetSiblingIndex(userInputLine.transform.GetSiblingIndex());

            TextMeshProUGUI[] texts = res.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
            {
                if (useTypewriterEffect)
                {
                    yield return StartCoroutine(TypewriterEffectWithColor(texts[0], interpretation[i]));
                }
                else
                {
                    texts[0].text = interpretation[i];
                }
            }
            else
            {
                Debug.LogError("Expected at least 1 TextMeshProUGUI component in responseLine prefab.");
            }

            userInputLine.transform.SetAsLastSibling();
            ScrollToBottom(i + 1);
        }
    }

    IEnumerator TypewriterEffectWithColor(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = ""; 
        int i = 0;
        while (i < fullText.Length)
        {
            if (fullText[i] == '<')
            {
                int tagEnd = fullText.IndexOf('>', i);
                if (tagEnd == -1) break;
                
                textComponent.text += fullText.Substring(i, tagEnd - i + 1);
                i = tagEnd + 1;
            }
            else
            {
                textComponent.text += fullText[i];
                i++;
            }

            yield return new WaitForSeconds(charDelay);
        }
    }

    void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2(0.0f, 100.0f);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }
    
    void AddEmptyLine()
    {
        RectTransform msgListRT = msgList.GetComponent<RectTransform>();
        msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

        GameObject emptyLine = Instantiate(responseLine, msgList.transform);
        emptyLine.transform.SetSiblingIndex(userInputLine.transform.GetSiblingIndex());

        TextMeshProUGUI[] emptyTexts = emptyLine.GetComponentsInChildren<TextMeshProUGUI>();
        if (emptyTexts.Length > 0)
        {
            emptyTexts[0].text = "";
        }
        else
        {
            Debug.LogError("Expected at least 1 TextMeshProUGUI component in responseLine prefab.");
        }
    }

    void ClearTerminal()
    {
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in msgList.transform)
        {
            if (child.gameObject != userInputLine)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
        
        RectTransform msgListRT = msgList.GetComponent<RectTransform>();
        msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, 0);
    }
}