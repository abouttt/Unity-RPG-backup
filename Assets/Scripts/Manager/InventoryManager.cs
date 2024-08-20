using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BaseManager<InventoryManager>
{
    private readonly Dictionary<Type, IInventory> _inventories = new();

    protected override void OnInit()
    {

    }

    protected override void OnClear()
    {
        _inventories.Clear();
    }

    protected override void OnDispose()
    {

    }

    public void Register<T>(T inventory) where T : IInventory
    {
        if (_inventories.ContainsKey(typeof(T)))
        {
            _inventories.Remove(typeof(T));
        }

        _inventories[typeof(T)] = inventory;
    }

    public void Unregister<T>(T inventory) where T : class, IInventory
    {
        if (_inventories.TryGetValue(typeof(T), out var value))
        {
            if (value == inventory)
            {
                _inventories.Remove(typeof(T));
            }
        }
    }

    public T Get<T>() where T : class, IInventory
    {
        if (_inventories.TryGetValue(typeof(T), out var inventory))
        {
            return inventory as T;
        }

        return null;
    }
}
