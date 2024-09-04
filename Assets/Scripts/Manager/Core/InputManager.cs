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

    protected override void OnInit()
    {
        _gameControls = new();
        _gameControls.UI.SetCallbacks(this);
        Enabled = false;
        CursorLocked = false;
    }

    protected override void OnClear()
    {
        Enabled = false;
    }

    protected override void OnDispose()
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

    public void OnItemInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_ItemInventoryPopup>(context);
    }

    public void OnEquipmentInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_EquipmentInventoryPopup>(context);
    }

    public void OnQuick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.IsActiveHelperPopup || Managers.UI.IsActiveSelfishPopup)
            {
                return;
            }

            int index = (int)context.ReadValue<float>();
            var quickable = Managers.UI.Get<UI_QuickInventoryFixed>().QuickInventoryRef.GetQuickable(index);
            if (quickable is Item item)
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.UseItem(item);
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.ActivePopupCount > 0)
            {
                Managers.UI.CloseTopPopup();
            }
        }
    }

    private void ShowOrClosePopup<T>(InputAction.CallbackContext context) where T : UI_Popup
    {
        if (context.performed)
        {
            if (Managers.UI.IsActiveHelperPopup)
            {
                return;
            }

            Managers.UI.ShowOrClose<T>();
        }
    }
}
