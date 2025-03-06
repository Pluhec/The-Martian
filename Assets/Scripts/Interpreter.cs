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
            response.Add("No command entered");
        }

        if (args[0] == "/help" || args[0] == "help")
        {
            response.Add("help" + "returns a list of commands");
            response.Add("exit" + "exits the terminal");
            response.Add("run " + "runs a program");
            response.Add("clear " + "clears the terminal");
            return response;
            return response;
        }
        else if (args[0] == "exit")
        {
            // Např. přesměrování na hlavní scénu nebo jiná akce
            response.Add("Exiting terminal...");
            return response;
        }
        else
        {
            response.Add("Unknown command: " + args[0]);
            response.Add("Type '/help' to see available commands");
            return response;
        }
    }
}