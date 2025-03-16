using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class Interpreter : MonoBehaviour
{
    private SolSystem solSystem;
    private QuestManager questManager;
    private TimeManager timeManager;

    private List<string> response = new List<string>();

    private void Awake()
    {
        solSystem = GameManager.Instance.SolSystem;
        questManager = GameManager.Instance.QuestManager;
        timeManager = GameManager.Instance.TimeManager;
    }
    
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
                    ("endsol", "Ends the current sol"),
                    ("hab", "Shows the hab stats"),
                    ("time", "Shows the current time"),
                    ("", ""),
                    ("listquests", "Lists all active quests"),
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
            
            case "exit":
                response.Add("EXIT_TERMINAL");
                break;
            
            case "ascii":
                LoadTitle("ascii.txt", "#FF0000", 0);
                break;
            
            case "endsol":
                if (questManager.AreAllQuestsCompleted())
                {
                    response.Add("END_SOL_SEQUENCE");
                }
                else
                {
                    response.Add("<color=#FF0000>Not all quests are completed!</color>");
                    response.AddRange(ListActiveQuests());
                }
                break;
            
            case "listquests":
                response.AddRange(ListActiveQuests());
                break;
            
            default:
                response.Add("<color=#FF0000>Unknown command: " + args[0] + "</color>");
                response.Add("Type <color=#00FF00>help</color> to see available commands");
                break;
        }
        return response;
    }
    
    public List<string> LoadAsciiLogo()
    {
        response.Clear();
        LoadTitle("ascii.txt", "#FF0000", 0);
        return new List<string>(response);
    }

    void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));

        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        while (!file.EndOfStream)
        {
            response.Add($"<color={color}>{file.ReadLine()}</color>");
        }
        
        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }
        
        file.Close();
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
                maxCommandLength = cmd.command.Length;
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
    
    private List<string> ListActiveQuests()
    {
        List<string> questList = new List<string>();
        List<Quest> quests = questManager.ActiveQuests;
        
        foreach (Quest quest in quests)
        {
            if (quest.isCompleted)
                questList.Add("<color=#00FF00>" + quest.questName + "</color>");
            else
                questList.Add("<color=#FF0000>" + quest.questName + "</color>");
        }
        return questList;
    }
    
    public List<string> GetGoodMorningMessage()
    {
        List<string> gmMessage = new List<string>();
        gmMessage.Add("Good morning, Commander!");
        gmMessage.Add("Current Time: " + timeManager.currentTime.ToShortTimeString());
        gmMessage.Add("SOL " + solSystem.currentSol);
        gmMessage.Add("Today's tasks:");
        foreach (Quest quest in questManager.ActiveQuests)
        {
            if (quest.isCompleted)
                gmMessage.Add("- <color=#00FF00>" + quest.questName + "</color>");
            else
                gmMessage.Add("- <color=#FF0000>" + quest.questName + "</color>");
        }
        return gmMessage;
    }
}