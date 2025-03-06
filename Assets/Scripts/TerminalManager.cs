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

    // Nastavení rychlosti typewriter efektu (znaky za sekundu)
    public float typewriterSpeed = 80f;
    private float charDelay { get { return 1f / typewriterSpeed; } }

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
        terminalInput.ActivateInputField();
        terminalInput.Select();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            // Uložení uživatelského vstupu
            string userInput = terminalInput.text;
            ClearInputField();

            // Přidání řádku s promptem a uživatelským vstupem nad input line
            AddDirectoryLine(userInput);

            // Interpretace vstupu a postupné zpracování odpovědních řádků
            List<string> interpretation = interpreter.Interpret(userInput);
            StartCoroutine(ProcessInterpreterLines(interpretation));

            // Vždy zajisti, že input line je na konci
            userInputLine.transform.SetAsLastSibling();

            // Aktivace a výběr vstupního pole
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
        // Vložíme řádek nad input line:
        msg.transform.SetSiblingIndex(userInputLine.transform.GetSiblingIndex());

        TextMeshProUGUI[] texts = msg.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 1)
        {
            // Index 0 obsahuje statický prompt (např. "C:\HAB>")
            // Index 1 se naplní uživatelským vstupem
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
            // Zvětšení výšky kontejneru zpráv pro nový řádek
            RectTransform msgListRT = msgList.GetComponent<RectTransform>();
            msgListRT.sizeDelta = new Vector2(msgListRT.sizeDelta.x, msgListRT.sizeDelta.y + 25.0f);

            GameObject res = Instantiate(responseLine, msgList.transform);
            // Vložíme nový řádek nad input line
            res.transform.SetSiblingIndex(userInputLine.transform.GetSiblingIndex());

            TextMeshProUGUI[] texts = res.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
            {
                // Postupně vypisujeme text s typewriter efektem
                yield return StartCoroutine(TypewriterEffectWithColor(texts[0], interpretation[i]));
            }
            else
            {
                Debug.LogError("Expected at least 1 TextMeshProUGUI component in responseLine prefab.");
            }
            
            // Po přidání řádku vždy zajistíme, že input line je na konci
            userInputLine.transform.SetAsLastSibling();
            ScrollToBottom(i + 1);
        }
    }

    IEnumerator TypewriterEffectWithColor(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = ""; // Začínáme s prázdným textem
        int i = 0;
        while (i < fullText.Length)
        {
            // Pokud narazíme na začátek HTML tagu (<), zpracujeme ho
            if (fullText[i] == '<')
            {
                int tagEnd = fullText.IndexOf('>', i);
                if (tagEnd == -1) break; // Pokud není konec tagu, ukončíme

                // Získání celého tagu (včetně < a >)
                string tag = fullText.Substring(i, tagEnd - i + 1);

                // Pokud je to barva, aplikujeme ji
                if (tag.StartsWith("<color="))
                {
                    // Extrahujeme barvu z tagu
                    string colorHex = tag.Substring(7, tag.Length - 8); // Např. #FF0000
                    Color color;
                    if (ColorUtility.TryParseHtmlString(colorHex, out color))
                    {
                        textComponent.color = color; // Nastavíme barvu
                    }
                }

                // Přeskočíme celý tag
                i = tagEnd + 1;
            }
            else
            {
                // Přidáme jeden znak
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