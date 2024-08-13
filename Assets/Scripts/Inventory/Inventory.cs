using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Inventory<T> where T : class
{
    [field: SerializeField]
    public int Capacity { get; private set; }

    [field: SerializeField, ReadOnly]
    public int Count { get; private set; }

    public IReadOnlyList<T> Items => _items;

    private List<T> _items;
    private readonly Dictionary<T, List<int>> _indexes = new();

    public void Init(int capacity = -1)
    {
        if (_items != null)
        {
            return;
        }

        if (capacity > 0)
        {
            Capacity = capacity;
        }

        var nullItems = Enumerable.Repeat<T>(null, Capacity);
        _items = new(nullItems);
    }

    public bool SetItem(T item, int index, int count)
    {
        if (item == null)
        {
            return false;
        }

        if (!IsIndexInRange(index))
        {
            return false;
        }

        if (count <= 0)
        {
            return false;
        }

        if (_items[index] != null)
        {
            RemoveItem(index);
        }

        _items[index] = item;
        AddItemIndex(item, index);
        Count++;

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

        RemoveItemIndex(_items[index], index);
        _items[index] = null;
        Count--;

        return true;
    }

    public U GetItem<U>(int index) where U : class, T
    {
        if (!IsIndexInRange(index))
        {
            return null;
        }

        return _items[index] as U;
    }

    public int GetItemIndex(T item)
    {
        if (item == null)
        {
            return -1;
        }

        if (_indexes.TryGetValue(item, out var indexes))
        {
            return indexes[0];
        }

        return -1;
    }

    public int[] GetItemAllIndex(T item)
    {
        if (item == null)
        {
            return null;
        }

        if (_indexes.TryGetValue(item, out var indexes))
        {
            return indexes.ToArray();
        }

        return null;
    }

    public int FindSameItemIndex(int startIndex, Predicate<T> match)
    {
        if (!IsIndexInRange(startIndex))
        {
            return -1;
        }

        return _items.FindIndex(startIndex, match);
    }

    public bool IsEmptyIndex(int index)
    {
        if (!IsIndexInRange(index))
        {
            return false;
        }

        return _items[index] == null;
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

    public bool IsIncluded(T item)
    {
        if (item == null)
        {
            return false;
        }

        return _indexes.ContainsKey(item);
    }

    public void SwapItem(int indexA, int indexB)
    {
        if (!IsIndexInRange(indexA) || !IsIndexInRange(indexB))
        {
            return;
        }

        var itemA = _items[indexA];
        if (itemA != null)
        {
            _indexes[itemA].Remove(indexA);
            _indexes[itemA].Add(indexB);
        }

        var itemB = _items[indexB];
        if (itemB != null)
        {
            _indexes[itemB].Remove(indexB);
            _indexes[itemB].Add(indexA);
        }

        (_items[indexA], _items[indexB]) = (_items[indexB], _items[indexA]);
    }

    public void AddCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            return;
        }

        var nullItems = Enumerable.Repeat<T>(null, capacity);
        _items.AddRange(nullItems);
    }

    public bool IsIndexInRange(int index)
    {
        return index >= 0 && index < _items.Count;
    }

    private void AddItemIndex(T item, int index)
    {
        if (!_indexes.ContainsKey(item))
        {
            _indexes.Add(item, new());
        }

        _indexes[item].Add(index);
    }

    private void RemoveItemIndex(T item, int index)
    {
        _indexes[item].Remove(index);

        if (_indexes[item].Count == 0)
        {
            _indexes.Remove(item);
        }
    }
}
