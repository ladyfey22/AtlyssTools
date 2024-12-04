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
        // test our delegate
        AtlyssToolsLoader.RegisterPreLibraryInit("AtlyssTools", () => Logger.LogInfo("PreLibraryInit delegate called"));

        new Harmony("AtlyssTools").PatchAll(Assembly.GetExecutingAssembly());

        // dump all loaded skills
        foreach (var skill in SkillManager.Instance.GetModded())
        {
            Logger.LogInfo($"Modded skill: {skill._skillName}");
        }

        // dump all loaded conditions
        foreach (var condition in ConditionManager.Instance.GetModded())
        {
            Logger.LogInfo($"Condition: {condition._conditionName}");
        }

        // add a general skill
        SkillManager.Instance.RegisterGeneralSkill("Hug");
    }
}