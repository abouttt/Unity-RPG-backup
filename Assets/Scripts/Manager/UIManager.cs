using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class UIManager : BaseManager<UIManager>
{
    public int ActivePopupCount => _activePopups.Count;
    public bool IsActiveHelperPopup => _helperPopup != null;
    public bool IsActiveSelfishPopup => _selfishPopup != null;

    private Transform _root;
    private readonly Dictionary<UIType, Transform> _roots = new();
    private readonly Dictionary<Type, BaseUI> _objects = new();
    private readonly LinkedList<PopupUI> _activePopups = new();
    private PopupUI _helperPopup;
    private PopupUI _selfishPopup;

    protected override void InitProcess()
    {
        var go = new GameObject($"UI_Root");
        Object.DontDestroyOnLoad(go);
        _root = go.transform;
    }

    protected override void ClearProcess()
    {
        foreach (var kvp in _roots)
        {
            foreach (Transform child in kvp.Value)
            {
                Object.Destroy(child.gameObject);
            }
        }

        _objects.Clear();
        _activePopups.Clear();
        _helperPopup = null;
        _selfishPopup = null;
    }

    protected override void DisposeProcess()
    {
        if (_root != null)
        {
            Object.Destroy(_root.gameObject);
        }
    }

    public T Get<T>() where T : BaseUI
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    public void Register<T>(T ui) where T : BaseUI
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
            InitPopup(ui as PopupUI);
        }

        ui.transform.SetParent(_roots[ui.UIType]);
        _objects.Add(typeof(T), ui);
    }

    public void Unregister<T>() where T : BaseUI
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            if (ui is PopupUI popup)
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

    public T Show<T>() where T : BaseUI
    {
        CheckInit();

        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.gameObject.activeSelf)
            {
                return ui as T;
            }

            if (ui is PopupUI popup)
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

                _activePopups.AddLast(popup);
                RefreshAllPopupDepth();
            }

            ui.gameObject.SetActive(true);

            return ui as T;
        }

        return null;
    }

    public bool IsActive<T>() where T : BaseUI
    {
        CheckInit();
        return _objects.TryGetValue(typeof(T), out var ui) && ui.gameObject.activeSelf;
    }

    public void Close<T>() where T : BaseUI
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
                var popup = ui as PopupUI;

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
            var popup = _activePopups.Last.Value;

            if (popup.IsHelper)
            {
                _helperPopup = null;
            }
            else if (popup.IsSelfish)
            {
                _selfishPopup = null;
            }

            _activePopups.RemoveLast();
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

    public void ShowOrClose<T>() where T : BaseUI
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

    private void InitPopup(PopupUI popup)
    {
        popup.gameObject.SetActive(false);
        popup.PopupRT.anchoredPosition = popup.DefaultPosition;
        popup.Focused += () =>
        {
            if (_activePopups.Last.Value == popup)
            {
                return;
            }

            _activePopups.Remove(popup);
            _activePopups.AddLast(popup);
            RefreshAllPopupDepth();
        };
    }

    private void RefreshAllPopupDepth()
    {
        int count = 0;
        foreach (var popup in _activePopups)
        {
            popup.Canvas.sortingOrder = (int)UIType.Top + count++;
        }
    }
}
