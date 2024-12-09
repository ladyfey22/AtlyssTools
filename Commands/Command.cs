using System;
using System.Linq;
using AtlyssTools.Utility;

namespace AtlyssTools.Commands;

public class CommandAttribute : Attribute
{
    // tag so that the command is automatically registered
}

public abstract class Command
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string[] Aliases { get; protected set; }

    /// <summary>
    ///     Executes the command, returns true if the command was successful.
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
        if (Instance != null) Plugin.Logger.LogWarning("CommandManager already exists");
        Instance = this;
    }

    public static CommandManager Instance { get; private set; }

    public bool ExecuteCommand(string commandName, string[] args)
    {
        // check each registered command list (commands are based on modid)
        foreach (var command in GetRegisteredList())
            if (command.Name == commandName || command.Aliases.Contains(commandName))
            {
                try
                {
                    if (command.Execute(ChatManager.Instance, args)) return true;
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError($"Error executing command '{commandName}': {e}");
                    return false;
                }

                command.DisplayUsage(ChatManager.Instance);
                return true;
            }

        ChatManager.Instance.SendMessage($"Command '{commandName}' not found");

        return true;
    }
}

[ChatProcessorAttribute]
public class CommandProcessor : ChatProcessor
{
    public override bool ProcessMessage(string message)
    {
        if (!string.IsNullOrEmpty(message) && message.StartsWith("/"))
        {
            var args = message.Substring(1).Split(' ').ToList();
            var commandName = args[0];
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
        Aliases = new[] { "?" };
    }

    public override bool Execute(ChatManager chatManager, string[] args)
    {
        if (args.Length > 0)
        {
            foreach (var command in CommandManager.Instance.GetRegisteredList())
                if (command.Name == args[0] || command.Aliases.Contains(args[0]))
                {
                    command.DisplayUsage(chatManager);
                    return true;
                }

            chatManager.SendMessage($"Command '{args[0]}' not found");
            return true;
        }

        foreach (var command in CommandManager.Instance.GetRegisteredList())
            chatManager.SendMessage($"{command.Name} - {command.Description}");

        return true;
    }

    public override bool DisplayUsage(ChatManager chatManager)
    {
        chatManager.SendMessage("Usage: /help [command]");
        return true;
    }
}

[CommandAttribute]
public class AtlyssToolsCommand : Command
{
    public AtlyssToolsCommand()
    {
        Name = "atlysstools";
        Description = "Gets the AtlyssTools version";
        Aliases = new[] { "at" };
    }

    public override bool Execute(ChatManager chatManager, string[] args)
    {
        // subcommands
        if (args.Length > 0)
        {
            if (args[0].Equals("dump"))
            {
                // dump scriptable objects
                AtlyssToolsLoader.Instance.GenerateDump();
            }
            else if (args[0].Equals("version"))
            {
                chatManager.SendMessage($"AtlyssTools version: {Plugin.Version}");
            }
            else
            {
                chatManager.SendMessage($"Unknown subcommand '{args[0]}'");
                // display usage
                DisplayUsage(chatManager);
            }
        }

        return true;
    }

    public override bool DisplayUsage(ChatManager chatManager)
    {
        chatManager.SendMessage("Usage: /atlysstools [dump|version]");
        return true;
    }
}