using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using UnityEngine;

namespace AtlyssTools.Commands;

public class CommandAttribute : System.Attribute
{
    // tag so that the command is automatically registered
}

public abstract class Command
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string[] Aliases { get; protected set; }

    /// <summary>
    /// Executes the command, returns true if the command was successful.
    /// </summary>
    /// <param name="chatManager">Chat manager instance</param>
    /// <param name="args">Command arguments (split by ' ') </param>
    /// <returns></returns>
    public abstract bool Execute(ChatManager chatManager, string[] args);
    public abstract bool DisplayUsage(ChatManager chatManager);
}

public class CommandManager : AttributeRegisterableManager<Command, CommandAttribute>
{
    
    
    public CommandManager()
    {
        if(Instance != null)
        {
            Plugin.Logger.LogWarning("CommandManager already exists");
        }
        Instance = this;
    }

    public bool ExecuteCommand(string commandName, string[] args)
    {
        // check each registered command list (commands are based on modid)
        
        foreach(var command in GetRegisteredList())
        {
            if (command.Name == commandName || command.Aliases.Contains(commandName))
            {
                if (command.Execute(ChatManager.Instance, args))
                {
                    return true;
                }

                command.DisplayUsage(ChatManager.Instance);
                return true;
            }
        }
        
        ChatManager.Instance.SendMessage($"Command '{commandName}' not found");

        return true;
    }

    public static CommandManager Instance { get; private set; }

}

[ChatProcessorAttribute]
public class CommandProcessor : ChatProcessor
{
    public override bool ProcessMessage(string message)
    {
        if(!string.IsNullOrEmpty(message) && message.StartsWith("/"))
        {
            List<string> args = message.Substring(1).Split(' ').ToList();
            string commandName = args[0];
            args.RemoveAt(0);
            return CommandManager.Instance.ExecuteCommand(commandName, args.ToArray());
        }

        return false;
    }
}

[CommandAttribute]
public class HelpCommand : Command
{
    public HelpCommand()
    {
        Name = "help";
        Description = "Displays a list of available commands";
        Aliases = new[] {"?"};
        
        Plugin.Logger.LogInfo("Help command registered");
    }

    public override bool Execute(ChatManager chatManager, string[] args)
    {
        foreach (var command in CommandManager.Instance.GetRegisteredList())
        {
            chatManager.SendMessage($"{command.Name} - {command.Description}");
        }

        return true;
    }

    public override bool DisplayUsage(ChatManager chatManager)
    {
        chatManager.SendMessage("Usage: /help");
        return true;
    }
}

[CommandAttribute]
public class AtlyssToolsCommand : Command
{
    public AtlyssToolsCommand()
    {
        Name = "atlysstools";
        Description = "Atlyss tools information";
        Aliases = new[] {"at"};
    }

    public override bool Execute(ChatManager chatManager, string[] args)
    {
        chatManager.SendMessage("Atlyss Tools Command");
        return true;
    }

    public override bool DisplayUsage(ChatManager chatManager)
    {
        chatManager.SendMessage("Usage: /at version  : Displays the version of Atlyss Tools");
        chatManager.SendMessage("Usage: /at help     : Displays this message");
        return true;
    }
}