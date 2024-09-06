using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Canvas<TSubitem> : UI_Base where TSubitem : UI_Subitem
{
    public readonly Dictionary<Type, TSubitem> _subitems = new();

    public void AddSubitem(TSubitem ui)
    {
        if (_subitems.ContainsKey(ui.GetType()))
        {
            Debug.LogWarning($"{ui.GetType()} has already been added to the canvas.");
        }
        else
        {
            ui.gameObject.SetActive(false);
            ui.transform.SetParent(transform);
            _subitems.Add(ui.GetType(), ui);
        }
    }

    public T GetSubitem<T>() where T : TSubitem
    {
        if (_subitems.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }
}
