using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

            // pridani prikazu a reset indexu
            if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != userInput)
            {
                commandHistory.Add(userInput);
            }
            historyIndex = commandHistory.Count; // posune index na konec

            AddDirectoryLine(userInput);

            List<string> interpretation = interpreter.Interpret(userInput);
            StartCoroutine(ProcessInterpreterLines(interpretation));

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
                // sipka nahoru - drivejsi command
                if (historyIndex > 0)
                {
                    historyIndex--;
                    terminalInput.text = commandHistory[historyIndex];
                    terminalInput.caretPosition = terminalInput.text.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // spika dolu - novejsi prikazy
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

    IEnumerator ProcessInterpreterLines(List<string> interpretation)
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
                yield return StartCoroutine(TypewriterEffectWithColor(texts[0], interpretation[i]));
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

                string tag = fullText.Substring(i, tagEnd - i + 1);

                if (tag.StartsWith("<color="))
                {
                    string colorHex = tag.Substring(7, tag.Length - 8);
                    Color color;
                    if (ColorUtility.TryParseHtmlString(colorHex, out color))
                    {
                        textComponent.color = color;
                    }
                }

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
}