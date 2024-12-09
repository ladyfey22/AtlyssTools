using System;
using AtlyssTools.Utility;

namespace AtlyssTools.Commands;

public abstract class ChatProcessor
{
    public virtual bool ProcessMessage(string message)
    {
        return false;
    }
}

public class ChatProcessorAttribute : Attribute
{
    // tag so that the command is automatically registered
}

public class ChatProcessorManager : AttributeRegisterableManager<ChatProcessor, ChatProcessorAttribute>
{
    public ChatProcessorManager()
    {
        if (Instance != null) Plugin.Logger.LogWarning("ChatProcessorManager already exists");
        Instance = this;
    }

    public static ChatProcessorManager Instance { get; private set; }
}

public class ChatManager
{
    private static ChatManager _instance;

    public static ChatManager Instance
    {
        get { return _instance ??= new(); }
    }

    public ChatBehaviour BaseGameChatManager { get; set; }

    public void SendMessage(string message)
    {
        // send message to chat
        BaseGameChatManager.New_ChatMessage(message);
    }

    public bool ProcessMessage(string message)
    {
        foreach (var processor in ChatProcessorManager.Instance.GetRegisteredList())
            if (processor.ProcessMessage(message))
                return true;

        return false;
    }
}