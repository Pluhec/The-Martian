using UnityEditor.Rendering.BuiltIn;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    List<string> response = new List<string>();
    
    public List<string> Interpret(string userInput)
    {
        response.Clear();
        
        string[] args = userInput.Split();
        
        if(agrs[0] == "/help" || args[0] == "help")
        {
            response.Add("Available commands:");
            response.Add("help -- show available commands");
            response.Add("exit -- exit the terminal");
            return response;
        }
        else if(args[0] == "exit")
        {
            // potom presun na hlavni scenu 
        }
        else
        {
            response.Add("Unknown command: " + args[0]);
            response.Add("Type '/help' to see available commands");
            return response;
        }
        
        // help --comands
    } 
}
