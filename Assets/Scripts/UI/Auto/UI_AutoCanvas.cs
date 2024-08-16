using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    public readonly Dictionary<Type, UI_Auto> _subitems = new();

    protected override void Init()
    {
        Managers.UI.Register(this);
    }

    public void AddSubitem(UI_Auto ui)
    {
        if (_subitems.ContainsKey(ui.GetType()))
        {
            Debug.LogWarning($"{ui.GetType()} has already been added to auto canvas.");
        }
        else
        {
            ui.gameObject.SetActive(false);
            ui.transform.SetParent(transform);
            _subitems.Add(ui.GetType(), ui);
        }
    }

    public T GetSubitem<T>() where T : UI_Auto
    {
        if (_subitems.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    private void OnDestroy()
    {
        _subitems.Clear();
    }
}
