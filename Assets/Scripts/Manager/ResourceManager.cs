using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public sealed class ResourceManager : BaseManager<ResourceManager>
{
    public int Count => _resources.Count;

    private readonly Dictionary<string, Object> _resources = new();

    protected override void InitProcess()
    {
        Addressables.InitializeAsync();
    }

    protected override void ClearProcess()
    {
        foreach (var kvp in _resources)
        {
            Addressables.Release(kvp.Value);
        }

        _resources.Clear();
        Resources.UnloadUnusedAssets();
    }

    protected override void DisposeProcess()
    {

    }

    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            return resource as T;
        }

        return null;
    }

    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            callback?.Invoke(resource as T);
        }
        else
        {
            Addressables.LoadAssetAsync<T>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (_resources.ContainsKey(key))
                    {
                        Addressables.Release(handle);
                    }
                    else
                    {
                        _resources.Add(key, handle.Result);
                    }

                    var resource = _resources[key];
                    callback?.Invoke(resource as T);
                }
                else
                {
                    Debug.LogWarning($"[ResourceManager/LoadAsync] Failed to load asset with key : {key}");
                }
            };
        }
    }

    public void LoadAllAsync<T>(string label, Action<T[]> callback = null) where T : Object
    {
        Addressables.LoadResourceLocationsAsync(label, typeof(T)).Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count == 0)
            {
                Debug.LogWarning($"[ResourceManager/LoadAllAsync] Failed to load asset with label : {label}");
            }
            else
            {
                int totalCount = handle.Result.Count;
                int loadedCount = 0;
                int index = 0;
                var resources = new T[totalCount];

                foreach (var result in handle.Result)
                {
                    LoadAsync<T>(result.PrimaryKey, resource =>
                    {
                        resources[index++] = resource;
                        if (++loadedCount == totalCount)
                        {
                            callback?.Invoke(resources);
                        }
                    });
                }
            }
        };
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        var prefab = Load<GameObject>(key);
        return pooling ? Managers.Pool.Pop(prefab, parent) : Object.Instantiate(prefab, parent);
    }

    public T Instantiate<T>(string key, Transform parent = null, bool pooling = false) where T : Component
    {
        var go = Instantiate(key, parent, pooling);
        return go.GetComponent<T>();
    }

    public void InstantiateAsync(string key, Action<GameObject> callback = null, Transform parent = null, bool pooling = false)
    {
        LoadAsync<GameObject>(key, prefab =>
        {
            var go = pooling ? Managers.Pool.Pop(prefab, parent) : Object.Instantiate(prefab, parent);
            callback?.Invoke(go);
        });
    }

    public void InstantiateAsync<T>(string key, Action<T> callback = null, Transform parent = null, bool pooling = false) where T : Component
    {
        InstantiateAsync(key, go => callback?.Invoke(go.GetComponent<T>()), parent, pooling);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        if (Managers.Pool.Push(go))
        {
            return;
        }

        Object.Destroy(go);
    }
}
