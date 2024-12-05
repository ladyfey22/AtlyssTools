using System.Reflection;
using AtlyssTools.Registries;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AtlyssTools;

[BepInPlugin("com.ladyfey22.atlysstools", "AtlyssTools", Version)]
[BepInProcess("Atlyss.exe")]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    private string _pluginPath;
    const string Version = "1.0.2";

    private void Awake()
    {
        _pluginPath = Paths.PluginPath + "/AtlyssTools";
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");


        AtlyssToolsLoader.LoadPlugin("AtlyssTools", _pluginPath);
        new Harmony("AtlyssTools").PatchAll(Assembly.GetExecutingAssembly());
    }
}