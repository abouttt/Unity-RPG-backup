using System;
using UnityEngine;

public class QuickInventory : MonoBehaviour
{
    public event Action<IQuickable, int> InventoryChanged;

    public int Capacity => _inventory.Capacity;

    [SerializeField]
    private Inventory<IQuickable> _inventory;

    private void Awake()
    {
        _inventory.Init();
    }

    public void SetQuickable(IQuickable quickable, int index)
    {
        if (_inventory.SetItem(quickable, index))
        {
            if (quickable is Item item)
            {
                item.Destroyed -= OnItemDestroyed;
                item.Destroyed += OnItemDestroyed;
            }

            InventoryChanged?.Invoke(quickable, index);
        }
    }

    public void RemoveQuickable(int index)
    {
        var quickable = _inventory[index];

        if (_inventory.RemoveItem(index))
        {
            if (quickable is Item item)
            {
                if (!_inventory.Contains(quickable))
                {
                    item.Destroyed -= OnItemDestroyed;
                }
            }

            InventoryChanged?.Invoke(null, index);
        }
    }

    public IQuickable GetQuickable(int index)
    {
        return _inventory.GetItem<IQuickable>(index);
    }

    public void SwapQuickable(int fromIndex, int toIndex)
    {
        _inventory.SwapItem(fromIndex, toIndex);
        InventoryChanged?.Invoke(_inventory[fromIndex], fromIndex);
        InventoryChanged?.Invoke(_inventory[toIndex], toIndex);
    }

    private void OnItemDestroyed(Item item)
    {
        for (int index = 0; index < _inventory.Capacity; index++)
        {
            if (_inventory[index] == item)
            {
                RemoveQuickable(index);
            }
        }
    }
}
