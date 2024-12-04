using System.Collections.Generic;

namespace AtlyssTools.Utility;

public interface LoaderStateManager
{
    void PreLibraryInit();
    void PostLibraryInit();
    void PreCacheInit();
    void PostCacheInit();
}

public class LoaderStateMachine
{
    public enum LoadState
    {
        PreCacheInit,
        PostCacheInit,
        PreLibraryInit,
        PostLibraryInit,
    }

    public LoadState State { get; private set; }

    internal void SetState(LoadState state)
    {
        State = state;

        foreach (var manager in _managers)
        {
            switch (state)
            {
                case LoadState.PreCacheInit:
                    manager.PreCacheInit();
                    break;
                case LoadState.PostCacheInit:
                    manager.PostCacheInit();
                    break;
                case LoadState.PreLibraryInit:
                    manager.PreLibraryInit();
                    break;
                case LoadState.PostLibraryInit:
                    manager.PostLibraryInit();
                    break;
            }
        }
    }


    public void RegisterManager(LoaderStateManager manager)
    {
        _managers.Add(manager);
    }

    public void RegisterManagers(IEnumerable<LoaderStateManager> managers)
    {
        _managers.AddRange(managers);
    }

    private readonly List<LoaderStateManager> _managers = new();
}