using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AtlyssTools;

[BepInPlugin("com.ladyfey22.atlysstools", "AtlyssTools", Version)]
[BepInProcess("Atlyss.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string Version = "1.0.4";
    internal new static ManualLogSource Logger;
    private string _pluginPath;

    private void Awake()
    {
        _pluginPath = Paths.PluginPath + "/AtlyssTools";
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Version} is loaded!");
        
        //UnityEngine.ResourcesAPI.overrideAPI = new Utility.AtlyssToolsResourceAPI();


        AtlyssToolsLoader.LoadPlugin("AtlyssTools", _pluginPath);
        AtlyssToolsLoader.FindAssetOnly();
        new Harmony("AtlyssTools").PatchAll(Assembly.GetExecutingAssembly());
    }
}