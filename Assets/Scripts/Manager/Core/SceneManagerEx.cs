using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public sealed class SceneManagerEx : BaseManager<SceneManagerEx>
{
    public event Action LoadCompleteReady;

    public BaseScene CurrentScene => Object.FindAnyObjectByType<BaseScene>();
    public string NextSceneAddress { get; private set; }
    public bool IsReadyToLoad { get; private set; }
    public bool IsReadyToLoadCompletion { get; private set; }
    public float LoadingProgress => _loadingUpdater.LoadingProgress;

    private AsyncOperationHandle<SceneInstance> _sceneHandle;
    private LoadingUpdater _loadingUpdater;

    protected override void OnInit()
    {
        Addressables.InitializeAsync();

        var go = Util.CreateGameObject(typeof(LoadingUpdater).Name, Managers.Instance.gameObject.transform);
        _loadingUpdater = go.AddComponent<LoadingUpdater>();
        _loadingUpdater.UpdateEnded += () =>
        {
            IsReadyToLoadCompletion = true;
            LoadCompleteReady?.Invoke();
        };
    }

    protected override void OnClear()
    {

    }

    protected override void OnDispose()
    {
        ClearLoadStatus();
        ReleaseSceneHandle();
        Object.Destroy(_loadingUpdater);
    }

    public void ReadyToLoad(string sceneAddress)
    {
        CheckInit();

        if (string.IsNullOrEmpty(sceneAddress))
        {
            Debug.LogWarning("[SceneManagerEx/ReadyToLoad] The scene address is empty");
            return;
        }

        if (IsReadyToLoad)
        {
            ClearLoadStatus();
        }

        NextSceneAddress = sceneAddress;
        IsReadyToLoad = true;

        if (!SceneManager.GetActiveScene().name.Equals("LoadingScene"))
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }

    public void StartLoad()
    {
        CheckInit();

        if (IsReadyToLoad)
        {
            ReleaseSceneHandle();
            _sceneHandle = Addressables.LoadSceneAsync(NextSceneAddress, LoadSceneMode.Single, false);
            _loadingUpdater.StartUpdate(_sceneHandle);
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx/StartLoad] Not ready to load");
        }
    }

    public void CompleteLoad()
    {
        CheckInit();

        if (IsReadyToLoadCompletion)
        {
            ClearLoadStatus();
            _sceneHandle.Result.ActivateAsync().allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx/CompleteLoad] Not ready to completion");
        }
    }

    private void ReleaseSceneHandle()
    {
        if (_sceneHandle.IsValid())
        {
            Addressables.Release(_sceneHandle);
        }
    }

    private void ClearLoadStatus()
    {
        LoadCompleteReady = null;
        NextSceneAddress = null;
        IsReadyToLoad = false;
        IsReadyToLoadCompletion = false;
    }
}
