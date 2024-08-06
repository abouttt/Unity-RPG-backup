using UnityEngine;

public class Managers : SingletonBehaviour<Managers>
{
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();

    protected override void Init()
    {
        base.Init();
        _input.Init();
        _pool.Init();
        _resource.Init();
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
        Resource.Clear();
    }

    protected override void Dispose()
    {
        base.Dispose();
        _input.Dispose();
        _pool.Dispose();
        _resource.Dispose();
    }
}
