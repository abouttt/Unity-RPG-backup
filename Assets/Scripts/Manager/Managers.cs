using UnityEngine;

public class Managers : SingletonBehaviour<Managers>
{
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;

    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();

    protected override void Init()
    {
        base.Init();
        _pool.Init();
        _resource.Init();
    }

    public static void Clear()
    {
        Pool.Clear();
        Resource.Clear();
    }

    protected override void Dispose()
    {
        base.Dispose();
        _pool.Dispose();
        _resource.Dispose();
    }
}
