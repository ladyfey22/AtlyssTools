using UnityEngine;

namespace AtlyssTools.Utility;

// WIP

public class AtlyssToolsResourceAPI : UnityEngine.ResourcesAPI
{
    // for now just log the call and pass it through
    public override Object Load(string path, System.Type type)
    {
        //Debug.Log($"AtlyssToolsResourceAPI.Load({path}, {type})");
        return base.Load(path, type);
    }
    
    // for now just log the call and pass it through
    public override Object[] LoadAll(string path, System.Type type)
    {
        //Debug.Log($"AtlyssToolsResourceAPI.LoadAll({path}, {type})");
        return base.LoadAll(path, type);
    }
    
    // for now just log the call and pass it through
    public override Shader FindShaderByName(string name)
    {
        //Debug.Log($"AtlyssToolsResourceAPI.FindShaderByName({name})");
        return base.FindShaderByName(name);
    }
    
    // for now just log the call and pass it through
    public override ResourceRequest LoadAsync(string path, System.Type type)
    {
        //Debug.Log($"AtlyssToolsResourceAPI.LoadAsync({path}, {type})");
        return base.LoadAsync(path, type);
    }
    
    public override void UnloadAsset(Object assetToUnload)
    {
        //Debug.Log($"AtlyssToolsResourceAPI.UnloadAsset({assetToUnload})");
        base.UnloadAsset(assetToUnload);
    }
}