using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PoolManager : BaseManager<PoolManager>
{
    #region Pool
    private class Pool
    {
        public Transform Root => _root;

        private GameObject _original;
        private readonly Transform _root;
        private readonly HashSet<GameObject> _actives = new();
        private readonly Stack<GameObject> _deactives = new();

        public Pool(GameObject original, int count)
        {
            _original = original;
            _root = new GameObject($"{original.name}_Root").transform;

            for (int i = 0; i < count; i++)
            {
                Create();
            }
        }

        public bool Push(GameObject go)
        {
            if (!_actives.Remove(go))
            {
                return false;
            }

            go.SetActive(false);
            go.transform.SetParent(_root);
            _deactives.Push(go);

            return true;
        }

        public void PushAll()
        {
            foreach (var go in _actives.ToArray())
            {
                Push(go);
            }

            _actives.Clear();
        }

        public GameObject Pop(Transform parent)
        {
            if (_deactives.Count == 0)
            {
                Create();
            }

            var go = _deactives.Pop();
            go.transform.SetParent(parent != null ? parent : _root);
            go.SetActive(true);
            _actives.Add(go);

            return go;
        }

        public void Clear()
        {
            foreach (var go in _actives)
            {
                Object.Destroy(go);
            }

            foreach (var go in _deactives)
            {
                Object.Destroy(go);
            }

            _actives.Clear();
            _deactives.Clear();
        }

        public void Dispose()
        {
            Clear();
            _original = null;
            Object.Destroy(_root.gameObject);
        }

        private void Create()
        {
            var go = Object.Instantiate(_original);
            go.SetActive(false);
            go.transform.SetParent(_root);
            go.name = _original.name;
            _deactives.Push(go);
        }
    }
    #endregion

    private Transform _root;
    private readonly Dictionary<string, Pool> _pools = new();

    protected override void InitProcess()
    {
        _root = Util.Instantiate("Pool_Root", true).transform;
    }

    protected override void ClearProcess()
    {
        foreach (var kvp in _pools.ToArray())
        {
            kvp.Value.Dispose();
        }

        _pools.Clear();
    }

    protected override void DisposeProcess()
    {
        if (_root != null)
        {
            Object.Destroy(_root.gameObject);
        }
    }

    public void CreatePool(GameObject original, int count = 0)
    {
        CheckInit();

        if (original == null)
        {
            Debug.LogWarning("[PoolManager/CreatePool] Original is null.");
            return;
        }

        if (_pools.ContainsKey(original.name))
        {
            Debug.LogWarning($"[PoolManager/CreatePool] {original.name} pool already exists.");
            return;
        }

        var pool = new Pool(original, count);
        pool.Root.SetParent(_root);
        _pools.Add(original.name, pool);
    }

    public bool Push(GameObject go)
    {
        CheckInit();

        if (go == null)
        {
            Debug.LogWarning($"[PoolManager/Push] GameObject is null.");
            return false;
        }

        if (!_pools.ContainsKey(go.name))
        {
            Debug.LogWarning($"[PoolManager/Push] {go.name} pool does not exist.");
            return false;
        }

        if (!_pools[go.name].Push(go))
        {
            Debug.LogWarning($"[PoolManager/Push] {go.name} is not included in this pool.");
            return false;
        }

        return true;
    }

    public void PushAll(GameObject go)
    {
        CheckInit();

        if (go == null)
        {
            Debug.LogWarning($"[PoolManager/PushAll] GameObject is null.");
            return;
        }

        PushAll(go.name);
    }

    public void PushAll(string name)
    {
        CheckInit();

        if (_pools.TryGetValue(name, out var pool))
        {
            pool.PushAll();
        }
        else
        {
            Debug.LogWarning($"[PoolManager/PushAll] {name} pool does not exist.");
        }
    }

    public GameObject Pop(GameObject go, Transform parent = null)
    {
        CheckInit();

        if (go == null)
        {
            Debug.LogWarning($"[PoolManager/Pop] GameObject is null.");
            return null;
        }

        if (!_pools.TryGetValue(go.name, out var pool))
        {
            CreatePool(go);
            pool = _pools[go.name];
        }

        return pool.Pop(parent);
    }

    public GameObject Pop(string name, Transform parent = null)
    {
        CheckInit();

        if (!_pools.TryGetValue(name, out var pool))
        {
            Debug.LogWarning($"[PoolManager/Pop] {name} pool does not exist.");
            return null;
        }

        return pool.Pop(parent);
    }

    public void ClearPool(GameObject go)
    {
        CheckInit();

        if (go == null)
        {
            Debug.LogWarning($"[PoolManager/ClearPool] GameObject is null.");
            return;
        }

        ClearPool(go.name);
    }

    public void ClearPool(string name)
    {
        CheckInit();

        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Clear();
        }
        else
        {
            Debug.LogWarning($"[PoolManager/ClearPool] {name} pool does not exist.");
        }
    }

    public void RemovePool(GameObject go)
    {
        CheckInit();

        if (go == null)
        {
            Debug.LogWarning($"[PoolManager/RemovePool] GameObject is null.");
            return;
        }

        RemovePool(go.name);
    }

    public void RemovePool(string name)
    {
        CheckInit();

        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Dispose();
            _pools.Remove(name);
        }
        else
        {
            Debug.LogWarning($"[PoolManager/RemovePool] {name} pool does not exist.");
        }
    }
}
