using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private string _defaultSceneAddress;

    protected override void Init()
    {
        base.Init();
        Managers.Clear();
        GC.Collect();

        if (!Managers.Scene.IsReadyToLoad)
        {
            Managers.Scene.ReadyToLoad(_defaultSceneAddress);
        }
    }

    private void Start()
    {
        LoadResourcesByLabels(Managers.Scene.StartLoad);
    }

    private void Update()
    {
        if (Managers.Scene.IsReadyToCompletion)
        {
            Managers.Scene.CompleteLoad();
        }
    }

    private void LoadResourcesByLabels(Action callback)
    {
        var loadResourceLabels = SceneSettings.Instance[Managers.Scene.NextSceneAddress].AddressableLabels;
        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            callback?.Invoke();
            return;
        }

        int totalCount = loadResourceLabels.Length;
        int loadedCount = 0;

        foreach (var label in loadResourceLabels)
        {
            Managers.Resource.LoadAllAsync<Object>(label.labelString, _ =>
            {
                if (++loadedCount == totalCount)
                {
                    callback?.Invoke();
                }
            });
        }
    }
}
