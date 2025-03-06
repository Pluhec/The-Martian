using UnityEngine;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    List<string> response = new List<string>();

    public List<string> Interpret(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        if (args.Length == 0)
        {
            response.Add("No command entered.");
            return response;
        }

        if (args[0] == "/help" || args[0] == "help")
        {
            response.Add("Available commands:");
            response.Add("help -- show available commands");
            response.Add("exit -- exit the terminal");
            return response;
        }
        else if (args[0] == "exit")
        {
            // Poté přesměruj na hlavní scénu nebo proveď potřebnou akci
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