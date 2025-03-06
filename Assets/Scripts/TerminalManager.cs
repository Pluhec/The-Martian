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
    }
}
