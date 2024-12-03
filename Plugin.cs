using System.Reflection;
using AtlyssTools.Registries;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AtlyssTools;

[BepInPlugin("com.ladyfey22.atlysstools", "AtlyssTools", "1.0.0")]
[BepInProcess("Atlyss.exe")]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    private string pluginPath;

    private void Awake()
    {
        pluginPath = Paths.PluginPath + "/AtlyssTools";
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        AtlyssToolsLoader.LoadPlugin("AtlyssTools", pluginPath);
        new Harmony("AtlyssTools").PatchAll(Assembly.GetExecutingAssembly());

        // all other initialization should be done by other mods loading themselves
    }
}