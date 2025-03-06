using UnityEngine;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    private Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        { "black", "#021b21" },
        { "gray", "#555d71" },    
        { "red", "#ff5879" },
        { "yellow", "#f2f1b9" },
        { "blue", "#9ed9d8" },
        { "purple", "#ed926f" },  
        { "orange", "#ef5847" }
    };
    
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
            ListEntry("help", "returns a list of commands");
            ListEntry("exit", "exits the terminal");
            ListEntry("run", "runs a program");
            ListEntry("clear", "clears the terminal");
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

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";
        return leftTag + s + rightTag;
    }

    void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["orange"]) + ": " + ColorString(b, colors["yellow"]));
    }
}