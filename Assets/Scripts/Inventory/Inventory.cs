using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory<T> where T : class
{
    [field: SerializeField]
    public int Capacity { get; private set; }

    [field: SerializeField, ReadOnly]
    public int Count { get; private set; }

    public IReadOnlyList<T> Items => _items.AsReadOnly();

    private List<T> _items;

    public void Init(int capacity = 0)
    {
        if (_items != null)
        {
            return;
        }

        if (capacity > 0)
        {
            Capacity = capacity;
        }

        _items = new(new T[Capacity]);
    }

    public bool SetItem(T item, int index)
    {
        if (item == null)
        {
            return false;
        }

        if (!IsIndexInRange(index))
        {
            return false;
        }

        if (_items[index] == null)
        {
            Count++;
        }

        _items[index] = item;

        return true;
    }

    public bool RemoveItem(int index)
    {
        if (!IsIndexInRange(index))
        {
            return false;
        }

        if (_items[index] == null)
        {
            return false;
        }

        _items[index] = null;
        Count--;

        return true;
    }

    public T GetItem(int index)
    {
        return IsIndexInRange(index) ? _items[index] : null;
    }

    public U GetItem<U>(int index) where U : class, T
    {
        return IsIndexInRange(index) ? _items[index] as U : null;
    }

    public bool SwapItem(int fromIndex, int toIndex)
    {
        if (!IsIndexInRange(fromIndex) || !IsIndexInRange(toIndex))
        {
            return false;
        }

        (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);

        return true;
    }

    public int GetItemIndex(T item)
    {
        return item != null ? _items.IndexOf(item) : -1;
    }

    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return IsIndexInRange(startIndex) ? _items.FindIndex(startIndex, match) : -1;
    }

    public int FindEmptyIndex(int startIndex)
    {
        if (!IsIndexInRange(startIndex))
        {
            return -1;
        }

        if (Capacity == Count)
        {
            return -1;
        }

        return _items.FindIndex(startIndex, item => item == null);
    }

    public bool HasItem(int index)
    {
        return IsIndexInRange(index) && _items[index] != null;
    }

    public bool IsIndexInRange(int index)
    {
        return index >= 0 && index < Capacity;
    }

    public void AddCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            return;
        }

        _items.AddRange(new T[capacity]);
        Capacity += capacity;
    }

    public void RemoveCapacity(int startIndex, int capacity)
    {
        if (!IsIndexInRange(startIndex))
        {
            return;
        }

        if (capacity <= 0)
        {
            return;
        }

        _items.RemoveRange(startIndex, capacity);
        Capacity -= capacity;
    }
}
