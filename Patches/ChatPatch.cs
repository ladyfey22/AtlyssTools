using AtlyssTools.Commands;
using HarmonyLib;
using UnityEngine;

namespace AtlyssTools.Patches;


[HarmonyPatch(typeof(ChatBehaviour), "Awake")]
public class ChatPatch
{
    // we want to get the ChatManager instance
    [HarmonyPostfix]
    public static void Postfix(ChatBehaviour __instance)
    {
        ChatManager.Instance.BaseGameChatManager = __instance;
    }
}

[HarmonyPatch(typeof(ChatBehaviour), "Send_ChatMessage")]
public class ChatSendMessagePatch
{
    [HarmonyPrefix]
    public static bool Prefix(ChatBehaviour __instance, string _message)
    {
        Plugin.Logger.LogInfo($"ChatBehaviour.Send_ChatMessage prefix patch: {_message}");
        if (string.IsNullOrEmpty(_message) || !ChatManager.Instance.ProcessMessage(_message))
        {
            return true;
        }
        
        ChatBehaviour behaviour = __instance;
        behaviour._lastMessage = _message;
        behaviour._chatInput.text = "";
        return false;
    }
}