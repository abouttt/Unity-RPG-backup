using UnityEngine;

public class Managers : SingletonBehaviour<Managers>
{
    public static CooldownManager Cooldown => Instance._cooldown;

    private readonly CooldownManager _cooldown = new();

    #region Core
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();
    #endregion

    private void LateUpdate()
    {
        _cooldown.UpdateCooldowns();
    }

    protected override void Init()
    {
        base.Init();
        _cooldown.Init();
        _input.Init();
        _pool.Init();
        _resource.Init();
        _scene.Init();
        _sound.Init();
        _ui.Init();
    }

    public static void Clear()
    {
        Cooldown.Clear();
        Input.Clear();
        Pool.Clear();
        Resource.Clear();
        Scene.Clear();
        Sound.Clear();
        UI.Clear();
    }

    protected override void Dispose()
    {
        base.Dispose();
        _cooldown.Dispose();
        _input.Dispose();
        _pool.Dispose();
        _resource.Dispose();
        _scene.Dispose();
        _sound.Dispose();
        _ui.Dispose();
    }
}
