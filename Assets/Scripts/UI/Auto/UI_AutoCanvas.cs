using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    public readonly Dictionary<Type, UI_Auto> _objects = new();

    protected override void Init()
    {
        Managers.UI.Register(this);
    }

    public void AddUniqueAutoUI<T>(T ui) where T : UI_Auto
    {
        if (_objects.ContainsKey(typeof(T)))
        {
            Debug.LogWarning($"{typeof(T)} has already been added to auto canvas.");
        }
        else
        {
            _objects.Add(typeof(T), ui);
        }
    }

    public T Get<T>() where T : UI_Auto
    {
        if (_objects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    private void OnDestroy()
    {
        _objects.Clear();
    }
}
