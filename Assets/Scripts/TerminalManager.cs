using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            // Uložení uživatelského vstupu
            string userInput = terminalInput.text;

            // Vyčištění inputu
            ClearInputField();

            // Vytvoření nového řádku s uživatelským vstupem
            AddDirectoryLine(userInput);

            // Interpretace uživatelského vstupu
            int lines = AddInterpreterLines(interpreter.Interpret(userInput));

            // Posun scrollu na konec
            ScrollToBottom(lines);

            // Přesun řádku uživatele na konec
            userInputLine.transform.SetAsLastSibling();

            // Refocus input fieldu
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    void ClearInputField()
    {
        terminalInput.text = "";
    }

    void AddDirectoryLine(string userInput)
    {
        // Zvětšení výšky kontejneru zpráv
        RectTransform msgListRT = msgList.GetComponent<RectTransform>();
        msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

        GameObject msg = Instantiate(directoryLine, msgList.transform);
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        Text[] texts = msg.GetComponentsInChildren<Text>();
        if (texts.Length > 0)
        {
            texts[0].text = userInput;
        }
        else
        {
            Debug.LogError("Expected at least 1 Text component in directoryLine prefab.");
        }
    }

    int AddInterpreterLines(List<string> interpretation)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            GameObject res = Instantiate(responseLine, msgList.transform);
            res.transform.SetAsLastSibling();

            RectTransform msgListRT = msgList.GetComponent<RectTransform>();
            msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

            Text[] texts = res.GetComponentsInChildren<Text>();
            if (texts.Length > 0)
            {
                texts[0].text = interpretation[i];
            }
            else
            {
                Debug.LogError("Expected at least 1 Text component in responseLine prefab.");
            }
        }
        return interpretation.Count;
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