using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class LoadingScene : BaseScene
{
    [field: SerializeField]
    public float NextSceneLoadDuration { get; private set; }

    [SerializeField]
    private string _defaultSceneAddress;

    protected override void Init()
    {
        base.Init();
        Managers.Clear();
        GC.Collect();

        Managers.Scene.LoadCompleteReady += () =>
        {
            StartCoroutine(LoadComplete());
        };

        if (!Managers.Scene.IsReadyToLoad)
        {
            Managers.Scene.ReadyToLoad(_defaultSceneAddress);
        }
    }

    private void Start()
    {
        LoadResourcesByLabels(Managers.Scene.StartLoad);
    }

    private IEnumerator LoadComplete()
    {
        yield return YieldCache.WaitForSeconds(NextSceneLoadDuration);
        Managers.Scene.CompleteLoad();
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
