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
            var commands = new List<(string command, string secondCommand)>
            {
                ("help", "Prints this message"),
                ("", ""),
                ("echo", "Prints a message"),
                ("clear", "Clears the terminal"),
                ("cls", "Clears the terminal"),
                ("color", "Changes the text color")
            };
            
            response.AddRange(FormatCommands(
                commands,
                commandColor: "#00FF00", 
                secondCommandColor: "#FFA500" 
            ));

            return response;
        }
        else if (args[0] == "echo")
        {
            response.Add("<color=#00FF00>echo</color>   | Prints a message");
            response.Add("<color=#FFA500>    | Example: echo hello world!</color>");
            return response;
        }
        else if (args[0] == "clear")
        {
            response.Add("<color=#00FF00>clear</color>  | Clears the terminal");
            return response;
        }
        else
        {
            response.Add("<color=#FF0000>Unknown command:</color> " + args[0]);
            response.Add("Type <color=#00FF00>/help</color> to see available commands");
            return response;
        }
    }

    // Metoda pro zarovnani techto car |
    private List<string> FormatCommands(
        List<(string command, string secondCommand)> commands,
        string commandColor = "#FFFFFF", 
        string secondCommandColor = "#FFFFFF" 
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
            if (string.IsNullOrEmpty(cmd.command) || string.IsNullOrEmpty(cmd.secondCommand))
            {
                formattedCommands.Add(""); 
                continue;
            }
            
            string paddedCommand = cmd.command.PadRight(maxCommandLength); 
            string spaces = new string(' ', 8); 
            string formattedLine = $"<color={commandColor}>{paddedCommand}</color>{spaces}| <color={secondCommandColor}>{cmd.secondCommand}</color>";
            formattedCommands.Add(formattedLine);
        }

        return formattedCommands;
    }
}