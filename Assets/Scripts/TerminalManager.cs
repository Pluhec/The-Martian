using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

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
        if(terminalInput.isFocused && terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            // Ulozeni uzivatelskeho vstupu
            string userInput = terminalInput.text;
            
            // vycisteni inputu
            ClearInputField();
            
            // Initiate a gameobject
            AddDirectoryLine(userInput);
            
            // Interpretace uzivatelskeho vstupu
            int lines = AddInterpreterLines(Interpreter.Interpret(userInput));
            
            // Scroll to the bottom
            ScrollToBottom(lines);

            // Moving the user line to the bottom
            userInputLine.transform.SetAsLastSibling();
            
            // Refocus the input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
            
        }
        
        void ClearInputField()
        {
            terminalInput.text = "";
        }
        
        void AddDirectoryLine(string userInput)
        {
            // Resize comandoveho konteineru
            Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 25.0f);

            GameObject msg = Instantiate(directoryLine, msgList.transform);
            
            msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

            msg.GetComponentsInChildren<Text>()[1].text = userInput;

        }

        int AddInterpreterLines(List<string> interpretation); 
        {
            for(int i = 0; i < interpretation.Count; i++)
            {
                gameObject res = Instantiate(responseLine, msgList.transform);
                
                res.transform.SetAsLastSibling();
                
                Vector2 ListSize = msgList.GetComponent<RectTransform>().sizeDelta;
                msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 25.0f);
                
                res.GetComponentsInChildren<Text>()[1].text = interpretation[i];
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
}
