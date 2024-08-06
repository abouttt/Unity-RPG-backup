using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public string SceneAddress { get; private set; }

    private void Awake()
    {
        if (SceneSettings.Instance[SceneAddress].ReloadSceneWhenNoResources && Managers.Resource.Count == 0)
        {
            Managers.Scene.ReadyToLoad(SceneAddress);
        }
        else
        {
            Init();
        }
    }

    protected virtual void Init()
    {
        if (FindObjectOfType(typeof(EventSystem)) == null)
        {
            Managers.Resource.InstantiateAsync("EventSystem.prefab");
        }
    }
}
