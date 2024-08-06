using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputManager : BaseManager<InputManager>
{
    public bool CursorLocked
    {
        get => _cursorLocked;
        set
        {
            _cursorLocked = value;
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private GameControls _gameControls;
    private bool _cursorLocked;

    protected override void InitProcess()
    {
        _gameControls = new();
        _gameControls.Enable();
        CursorLocked = false;
    }

    protected override void ClearProcess()
    {

    }

    protected override void DisposeProcess()
    {
        _gameControls.Disable();
        _gameControls.Dispose();
    }

    public InputActionMap GetActionMap(string nameOrId, bool throwIfNotFound = false)
    {
        return _gameControls.asset.FindActionMap(nameOrId, throwIfNotFound);
    }

    public InputAction GetAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return _gameControls.FindAction(actionNameOrId, throwIfNotFound);
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        string key = GetAction(actionNameOrId).bindings[bindingIndex].path;
        string path = key.GetStringAfterLastSlash();
        return path.ToUpper();
    }
}
