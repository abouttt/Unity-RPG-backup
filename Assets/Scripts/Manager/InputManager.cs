using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputManager : BaseManager<InputManager>, GameControls.IUIActions
{
    public bool Enabled
    {
        get => _gameControls.asset.enabled;
        set
        {
            if (value)
            {
                _gameControls.Enable();
            }
            else
            {
                _gameControls.Disable();
            }
        }
    }

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
        _gameControls.UI.SetCallbacks(this);
        Enabled = false;
        CursorLocked = false;
    }

    protected override void ClearProcess()
    {
        Enabled = false;
    }

    protected override void DisposeProcess()
    {
        _gameControls.UI.RemoveCallbacks(this);
        _gameControls.Dispose();
    }

    public InputActionMap GetActionMap(string nameOrId, bool throwIfNotFound = false)
    {
        CheckInit();
        return _gameControls.asset.FindActionMap(nameOrId, throwIfNotFound);
    }

    public InputAction GetAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        CheckInit();
        return _gameControls.FindAction(actionNameOrId, throwIfNotFound);
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        CheckInit();
        string key = GetAction(actionNameOrId).bindings[bindingIndex].path;
        string path = key.GetStringAfterLastSlash();
        return path.ToUpper();
    }

    public void OnCursorToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CursorLocked = !CursorLocked;
        }
    }
}
