using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public sealed class SceneManagerEx : BaseManager<SceneManagerEx>
{
    public BaseScene CurrentScene
    {
        get
        {
            if (_currentScene == null)
            {
                _currentScene = Object.FindAnyObjectByType<BaseScene>();
            }

            return _currentScene;
        }
    }

    public string NextSceneAddress { get; private set; }
    public bool IsReadyToLoad { get; private set; }
    public bool IsReadyToCompletion { get; private set; }
    public float LoadingProgress => _sceneHandle.PercentComplete;

    private AsyncOperationHandle<SceneInstance> _sceneHandle;
    private BaseScene _currentScene;

    protected override void InitProcess()
    {
        Addressables.InitializeAsync();
    }

    protected override void ClearProcess()
    {

    }

    protected override void DisposeProcess()
    {
        ClearLoadStatus();
        if (_sceneHandle.IsValid())
        {
            Addressables.Release(_sceneHandle);
        }
    }

    public void ReadyToLoad(string sceneAddress)
    {
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
        if (IsReadyToLoad)
        {
            if (_sceneHandle.IsValid())
            {
                Addressables.Release(_sceneHandle);
            }

            _sceneHandle = Addressables.LoadSceneAsync(NextSceneAddress, LoadSceneMode.Single, false);
            _sceneHandle.Completed += handle => IsReadyToCompletion = true;
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx/StartLoad] Not ready to load");
        }
    }

    public void CompleteLoad()
    {
        if (IsReadyToCompletion)
        {
            ClearLoadStatus();
            _sceneHandle.Result.ActivateAsync().allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx/CompleteLoad] Not ready to completion");
        }
    }

    private void ClearLoadStatus()
    {
        NextSceneAddress = null;
        IsReadyToLoad = false;
        IsReadyToCompletion = false;
    }
}
