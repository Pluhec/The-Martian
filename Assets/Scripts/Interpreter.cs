using UnityEngine;
using System.Collections.Generic;
using System;

public class Interpreter : MonoBehaviour
{
    List<string> response = new List<string>();

    public List<string> Interpret(string userInputText)
    {
        response.Clear();
        string[] args = userInputText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (args.Length == 0)
        {
            response.Add("<color=#FF0000>No command entered</color>");
            return response;
        }
        
        switch (args[0].ToLower())
        {
            case "help":
                var commands = new List<(string command, string description)>
                {
                    ("help", "Prints this message"),
                    ("", ""),
                    ("echo", "Prints a message"),
                    ("clear", "Clears the terminal"),
                    ("", ""),
                    ("rover", "Shows the rover stats"),
                    ("endSol", "Ends the current sol"),
                    ("hab", "Shows the hab stats"),
                    ("time", "Shows the current time"),
                    ("", ""),
                    ("exit", "Exits the terminal")
                };
                response.AddRange(FormatCommands(commands, "#00FF00", "#FFA500"));
                break;

            case "echo":
                if (args.Length > 1)
                {
                    string message = string.Join(" ", args, 1, args.Length - 1);
                    response.Add(message);
                }
                else
                {
                    response.Add("<color=#FF0000>Error: No message provided for echo command.</color>");
                }
                break;

            case "clear":
            case "cls":
                response.Add("CLEAR_TERMINAL");
                break;

            default:
                response.Add("<color=#FF0000>Unknown command:</color> " + args[0]);
                response.Add("Type <color=#00FF00>help</color> to see available commands");
                break;
        }

        return response;
    }

    private List<string> FormatCommands(
        List<(string command, string description)> commands,
        string commandColor = "#FFFFFF", 
        string descriptionColor = "#FFFFFF" 
    )
    {
        List<string> formattedCommands = new List<string>();

        int maxCommandLength = 0;
        foreach (var cmd in commands)
        {
            if (!string.IsNullOrEmpty(cmd.command) && cmd.command.Length > maxCommandLength)
            {
                maxCommandLength = cmd.command.Length;
            }
        }
        
        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd.command) || string.IsNullOrEmpty(cmd.description))
            {
                formattedCommands.Add("");
                continue;
            }
            
            string paddedCommand = cmd.command.PadRight(maxCommandLength);
            string spaces = new string(' ', 8);
            string formattedLine = $"<color={commandColor}>{paddedCommand}</color>{spaces}| <color={descriptionColor}>{cmd.description}</color>";
            formattedCommands.Add(formattedLine);
        }

        return formattedCommands;
    }
}