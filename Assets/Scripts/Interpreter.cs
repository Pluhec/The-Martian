using UnityEngine;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    List<string> response = new List<string>();

    public List<string> Interpret(string userInputText)
    {
        response.Clear();
        string[] args = userInputText.Split();

        if (args.Length == 0)
        {
            response.Add("<color=#FF0000>No command entered</color>");
        }

        if (args[0] == "/help" || args[0] == "help")
        {
            response.Add("<color=#00FF00>help</color> returns a list of commands");
            response.Add("<color=#00FF00>exit</color> exits the terminal");
            response.Add("<color=#00FF00>run</color> runs a program");
            response.Add("<color=#00FF00>clear</color> clears the terminal");
            return response;
        }
        else if (args[0] == "exit")
        {
            // Např. přesměrování na hlavní scénu nebo jiná akce
            response.Add("<color=#FFFF00>Exiting terminal...</color>");
            return response;
        }
        else
        {
            response.Add("<color=#FF0000>Unknown command:</color> " + args[0]);
            response.Add("Type <color=#00FF00>/help</color> to see available commands");
            return response;
        }
    }
}