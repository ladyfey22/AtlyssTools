using System;
using System.Collections.Generic;
using AtlyssTools.Utility;
using UnityEngine;

namespace AtlyssTools.Commands;


public abstract class ChatProcessor
{
    public virtual bool ProcessMessage(string message)
    {
        return false;
    }
}

public class ChatProcessorAttribute : System.Attribute
{
    // tag so that the command is automatically registered
}

public class ChatProcessorManager : AttributeRegisterableManager<ChatProcessor, ChatProcessorAttribute>
{
    public ChatProcessorManager()
    {
        if(Instance != null)
        {
            Plugin.Logger.LogWarning("ChatProcessorManager already exists");
        }
        Instance = this;
    }
    
    public static ChatProcessorManager Instance { get; private set; }
}



public class ChatManager
{
    
    public void SendMessage(string message)
    {
        // send message to chat
        BaseGameChatManager.New_ChatMessage(message);
    }
    
    public bool ProcessMessage(string message)
    {
        foreach(var processor in ChatProcessorManager.Instance.GetRegisteredList())
        {
            if (processor.ProcessMessage(message))
            {
                return true;
            }
        }
        
        return false;
    }

    public static ChatManager Instance
    {
        get
        {
            return _instance ??= new();
        }
    }

    private static ChatManager _instance;
    
    public ChatBehaviour BaseGameChatManager { get; set; }
}