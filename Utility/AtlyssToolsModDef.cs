namespace AtlyssTools.Utility;

// an atlysstools json file, marking the mod as an atlysstools mod even if there is no assembly. this must be in the root of the mod folder
public class AtlyssToolsModDef
{
    public string AtlasVersion; // the version of the game this mod is compatible with (minimum version)
    public string Author;
    public string Description;
    public string ModId;
    public string ModName;
    public string Version;
}