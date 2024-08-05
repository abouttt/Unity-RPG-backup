using UnityEngine;

public class Managers : SingletonBehaviour<Managers>
{
    public static PoolManager Pool => Instance._pool;

    private readonly PoolManager _pool = new();

    protected override void Init()
    {
        base.Init();
        _pool.Init();
    }

    public static void Clear()
    {
        Pool.Clear();
    }

    protected override void Dispose()
    {
        base.Dispose();
        _pool.Dispose();
    }
}
