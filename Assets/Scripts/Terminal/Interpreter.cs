using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class Interpreter : MonoBehaviour
{
    private SolSystem solSystem;
    private QuestManager questManager;
    private TimeManager timeManager;

    [Header("Pathfinder Message System")]
    [Tooltip("Dictionary of messages associated with each pathfinder quest")]
    private Dictionary<int, string> pathfinderMessages = new Dictionary<int, string>();

    private List<string> response = new List<string>();

    private void Awake()
    {
        solSystem = GameManager.Instance.SolSystem;
        questManager = GameManager.Instance.QuestManager;
        timeManager = GameManager.Instance.TimeManager;
        
        // Initialize pathfinder message dictionary
        InitializePathfinderMessages();
    }
    
    private void InitializePathfinderMessages()
    {
        // Add messages for pathfinder quests
        // Quest IDs are even numbers (10, 12, 14, etc.)
        // Terminal quest IDs are odd numbers (11, 13, 15, etc.)
        pathfinderMessages.Add(10, "System initialized. First contact established.");
        pathfinderMessages.Add(12, "Life support systems operational. Oxygen levels stabilized.");
        pathfinderMessages.Add(14, "Resource management protocols enabled. Survival mode activated.");
        pathfinderMessages.Add(16, "Planetary scan complete. Mars surface data analyzed.");
        pathfinderMessages.Add(18, "Distress signal received. Rescue mission coordinated.");
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
                    ("message", "Confirms pathfinder message completion"),
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
            
            case "nemec":
                LoadTitle("nemec.txt", "#ffffff", 0);
                break;
            
            case "vrablik":
                LoadTitle("vrablik.txt", "#ffffff", 0);
                break;
            
            case "svejda":
                LoadTitle("svejda.txt", "#ffffff", 0);
                break; 
            
            case "time":
                response.Add("Current time: " + timeManager.GetFormattedTime());
                break;
            
            case "message":
                ProcessMessageCommand();
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
    
    private void ProcessMessageCommand()
    {
        // Find the next terminal quest (odd numbers) that is not completed
        foreach (Quest quest in questManager.ActiveQuests)
        {
            // Check if this is a terminal quest (odd quest ID)
            if (!quest.isCompleted && quest.questID % 2 == 1)
            {
                // Get the associated pathfinder quest ID (even number)
                int pathfinderQuestID = quest.questID - 1;
                
                // Check if the previous pathfinder quest was completed
                Quest pathfinderQuest = questManager.ActiveQuests.Find(q => q.questID == pathfinderQuestID);
                
                if (pathfinderQuest != null && pathfinderQuest.isCompleted)
                {
                    // Complete this terminal quest
                    questManager.MarkQuestAsCompletedByID(quest.questID);
                    
                    // Display the message
                    if (pathfinderMessages.TryGetValue(pathfinderQuestID, out string message))
                    {
                        response.Add("<color=#00FF00>Message confirmation:</color>");
                        response.Add("<color=#00FFFF>" + message + "</color>");
                        response.Add("<color=#00FF00>Quest completed! Continue to next objective.</color>");
                    }
                    else
                    {
                        response.Add("<color=#00FF00>Message confirmed. Quest completed!</color>");
                    }
                    
                    return;
                }
            }
        }
        
        // If we got here, no eligible terminal quest was found
        response.Add("<color=#FF0000>No pending messages to confirm.</color>");
        response.Add("<color=#FFFF00>Complete a pathfinder sequence first.</color>");
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
        gmMessage.Add("SOL " + solSystem.currentSol + "; " + timeManager.GetFormattedTime());
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