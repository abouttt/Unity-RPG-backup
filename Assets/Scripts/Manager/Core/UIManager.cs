using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class UIManager : BaseManager<UIManager>
{
    public int Count => _objects.Count;
    public int ActivePopupCount => _activePopups.Count;
    public bool IsActiveHelperPopup => _helperPopup != null;
    public bool IsActiveSelfishPopup => _selfishPopup != null;

    private Transform _root;
    private readonly Dictionary<UIType, Transform> _roots = new();
    private readonly Dictionary<Type, UI_Base> _objects = new();
    private readonly LinkedList<UI_Popup> _activePopups = new();
    private UI_Popup _helperPopup;
    private UI_Popup _selfishPopup;

    protected override void OnInit()
    {
        _root = Util.CreateGameObject("UI_Root", Managers.Instance.gameObject.transform).transform;

        foreach (UIType type in Enum.GetValues(typeof(UIType)))
        {
            if (type == UIType.Subitem)
            {
                continue;
            }

            var root = new GameObject($"{type}_Root").transform;
            root.SetParent(_root);
            _roots.Add(type, root);
        }
    }

    protected override void OnClear()
    {
        foreach (var kvp in _roots)
        {
            if (kvp.Key == UIType.Global)
            {
                continue;
            }

            foreach (Transform child in kvp.Value)
            {
                Object.Destroy(child.gameObject);
            }
        }

        List<Type> removals = new();
        foreach (var ui in _objects.Values)
        {
            if (ui.UIType != UIType.Global)
            {
                removals.Add(ui.GetType());
            }
        }
        foreach (var ui in removals)
        {
            _objects.Remove(ui);
        }

        _activePopups.Clear();
        _helperPopup = null;
        _selfishPopup = null;
    }

    protected override void OnDispose()
    {
        foreach (var kvp in _roots)
        {
            foreach (Transform child in kvp.Value)
            {
                Object.Destroy(child.gameObject);
            }
        }

        _objects.Clear();
        Object.Destroy(_root.gameObject);
    }

    public T Get<T>() where T : UI_Base
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    public void Register<T>(T ui) where T : UI_Base
    {
        CheckInit();

        if (ui == null)
        {
            Debug.LogWarning($"[UIManager/Register] {typeof(T)} object is null.");
            return;
        }

        if (ui.UIType == UIType.Subitem)
        {
            Debug.LogWarning($"[UIManager/Register] Subitem type can't register : {ui.name}");
            return;
        }

        if (_objects.ContainsKey(typeof(T)))
        {
            Debug.LogWarning($"[UIManager/Register] {ui.name} is already registered.");
            return;
        }

        if (!ui.TryGetComponent<Canvas>(out var canvas))
        {
            Debug.LogWarning($"[UIManager/Register] {ui.name} is no exist Canvas component.");
            return;
        }
        else
        {
            canvas.sortingOrder = (int)ui.UIType;
        }

        if (ui.UIType == UIType.Popup)
        {
            InitPopup(ui as UI_Popup);
        }

        ui.transform.SetParent(_roots[ui.UIType]);
        _objects.Add(typeof(T), ui);
    }

    public void Unregister<T>() where T : UI_Base
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            if (ui is UI_Popup popup)
            {
                popup.ClearEvents();

                if (popup.IsHelper && _helperPopup == popup)
                {
                    _helperPopup = null;
                }
                else if (popup.IsSelfish && _selfishPopup == popup)
                {
                    _selfishPopup = null;
                }

                _activePopups.Remove(popup);
            }

            _objects.Remove(typeof(T));
        }
        else
        {
            Debug.LogWarning($"[UIManager/Unregister] {typeof(T)} is not registered.");
        }
    }

    public bool Contains<T>() where T : UI_Base
    {
        return _objects.ContainsKey(typeof(T));
    }

    public T Show<T>() where T : UI_Base
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.gameObject.activeSelf)
            {
                return ui as T;
            }

            if (ui is UI_Popup popup)
            {
                if (IsActiveSelfishPopup && !popup.IgnoreSelfish)
                {
                    return null;
                }

                if (popup.IsHelper)
                {
                    if (IsActiveHelperPopup)
                    {
                        _activePopups.Remove(_helperPopup);
                        _helperPopup.gameObject.SetActive(false);
                    }

                    _helperPopup = popup;
                }
                else if (popup.IsSelfish)
                {
                    CloseAll(UIType.Popup);
                    _selfishPopup = popup;
                }

                _activePopups.AddFirst(popup);
                RefreshAllPopupDepth();
            }

            ui.gameObject.SetActive(true);

            return ui as T;
        }

        return null;
    }

    public bool IsActive<T>() where T : UI_Base
    {
        CheckInit();
        return _objects.TryGetValue(typeof(T), out var ui) && ui.gameObject.activeSelf;
    }

    public void Close<T>() where T : UI_Base
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            if (!ui.gameObject.activeSelf)
            {
                return;
            }

            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;

                if (popup.IsHelper)
                {
                    _helperPopup = null;
                }
                else if (popup.IsSelfish)
                {
                    _selfishPopup = null;
                }

                _activePopups.Remove(popup);
            }

            ui.gameObject.SetActive(false);
        }
    }

    public void CloseTopPopup()
    {
        CheckInit();

        if (ActivePopupCount > 0)
        {
            var popup = _activePopups.First.Value;

            if (popup.IsHelper)
            {
                _helperPopup = null;
            }
            else if (popup.IsSelfish)
            {
                _selfishPopup = null;
            }

            _activePopups.RemoveFirst();
            popup.gameObject.SetActive(false);
        }
    }

    public void CloseAll(UIType type)
    {
        CheckInit();

        if (type == UIType.Popup)
        {
            foreach (var popup in _activePopups)
            {
                popup.gameObject.SetActive(false);
            }

            _activePopups.Clear();
            _helperPopup = null;
            _selfishPopup = null;
        }
        else
        {
            foreach (Transform child in _roots[type])
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ShowOrClose<T>() where T : UI_Base
    {
        CheckInit();

        if (IsActive<T>())
        {
            Close<T>();
        }
        else
        {
            Show<T>();
        }
    }

    private void InitPopup(UI_Popup popup)
    {
        popup.Focused += () =>
        {
            if (_activePopups.First.Value == popup)
            {
                return;
            }

            _activePopups.Remove(popup);
            _activePopups.AddFirst(popup);
            RefreshAllPopupDepth();
        };
    }

    private void RefreshAllPopupDepth()
    {
        int count = 1;
        foreach (var popup in _activePopups)
        {
            popup.Canvas.sortingOrder = (int)UIType.Top - count++;
        }
    }
}
