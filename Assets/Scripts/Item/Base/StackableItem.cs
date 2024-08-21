using System;
using UnityEngine;

public abstract class StackableItem : Item, IStackableItem
{
    public event Action CountChanged;

    public StackableItemData StackableData => Data as StackableItemData;

    public int Count
    {
        get => _count;
        set
        {
            int prevCount = _count;
            _count = Mathf.Clamp(value, 0, MaxCount);
            if (_count != prevCount)
            {
                CountChanged?.Invoke();
            }
        }
    }

    public int MaxCount => StackableData.MaxCount;
    public bool IsMax => _count >= MaxCount;
    public bool IsEmpty => _count <= 0;

    private int _count;

    public StackableItem(StackableItemData stackableData, int count)
        : base(stackableData)
    {
        Count = count;
    }

    public int AddCountAndGetExcess(int count)
    {
        int nextCount = _count + count;
        Count += count;
        return nextCount > MaxCount ? nextCount - MaxCount : 0;
    }
}
