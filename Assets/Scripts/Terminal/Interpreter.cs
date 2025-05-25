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

    private Dictionary<int, string> solLoreMessages = new Dictionary<int, string>()
    {
        {
            21, "The Hab is experiencing unstable power. Clean the solar panels to ensure sufficient energy for life support."
        },
        {
            24, "To begin preparing the Hab for sustainable life on Mars, the interior must first be cleared of clutter and unnecessary equipment. With a clean and open space, a layer of Martian soil can then be spread across the floor — laying the foundation for future plant growth and self-sufficiency."
        },
        {
            30, "With the floor now layered with Martian soil, the next phase begins: growing food. Potatoes stored in the Hab can be used to initiate cultivation. But Martian dirt alone won’t be enough — to support growth, fertilizer must be added. Outside the Hab, sealed containers with organic waste can be retrieved and processed at the fertilizer station."
        },
        {
            36, "Water reserves in the Hab are running dangerously low. Without a steady supply, neither life support nor farming can continue. A backup water barrel is stored in the landing module. Once retrieved, the water must be used wisely — by assembling the heating system to extract liquid from the reclaimed materials."
        },
        {
            94, "Communication with NASA is still down, and the Hab’s systems are isolated. A potential solution lies in Pathfinder — an old rover from a previous mission. If it can be recovered, it might serve as a link to Earth. But first, it has to be located, cleared of debris, and carefully reactivated."
        },
        {
            100, "With communication restored, NASA has transmitted coordinates and a secure PIN to unlock the emergency return module. Everything has led to this moment. The path is clear — reach the escape vehicle, authenticate the code, and leave Mars behind."
        }
    };

    private List<string> response = new List<string>();

    private void Awake()
    {
        solSystem = GameManager.Instance.SolSystem;
        questManager = GameManager.Instance.QuestManager;
        timeManager = GameManager.Instance.TimeManager;
        
        InitializePathfinderMessages();
    }
    
    private void InitializePathfinderMessages()
    {
        pathfinderMessages.Add(93, "We hear you loud and clear, Mark. Coordinating your return as we speak. Stay safe out there.");
        pathfinderMessages.Add(95, "Fuel tanks are full and all systems are green. The rocket is prepped and ready for launch whenever you are.");
        pathfinderMessages.Add(97, "The launch PIN is 4832. Once you have it, begin preparations immediately. We're all rooting for you.");
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
        foreach (Quest quest in questManager.ActiveQuests)
        {
            if (!quest.isCompleted && quest.questID % 2 == 0)
            {
                int pathfinderQuestID = quest.questID - 1;
                Quest pfq = questManager.ActiveQuests
                    .Find(q => q.questID == pathfinderQuestID);

                if (pfq != null && pfq.isCompleted)
                {
                    questManager.MarkQuestAsCompletedByID(quest.questID);

                    if (pathfinderMessages.TryGetValue(pathfinderQuestID, out string msg))
                        response.Add("<color=#00FFFF>" + msg + "</color>");
                    else
                        response.Add("<color=#00FF00>Message confirmed. Quest completed!</color>");

                    return;
                }
            }
        }

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
        
        if (solLoreMessages.TryGetValue(solSystem.currentSol, out string lore))
        {
            gmMessage.Add(""); 
            foreach (string line in WrapText(lore, 61))
            {
                gmMessage.Add("<i>" + line + "</i>");
            }
        }

        return gmMessage;
    }
    
    private List<string> WrapText(string text, int maxLineLength)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;

        foreach (var word in words)
        {
            if ((currentLine.Length + word.Length + (currentLine.Length > 0 ? 1 : 0)) <= maxLineLength)
            {
                if (currentLine.Length > 0)
                    currentLine += " ";
                currentLine += word;
            }
            else
            {
                if (currentLine.Length > 0)
                    lines.Add(currentLine);
                currentLine = word;
            }
        }
        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return lines;
    }
}