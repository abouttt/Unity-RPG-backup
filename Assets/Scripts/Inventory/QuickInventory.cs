using System;
using UnityEngine;

public class QuickInventory : MonoBehaviour
{
    public event Action<IQuickable, int> InventoryChanged;

    [SerializeField]
    private Inventory<IQuickable> _inventory;

    private void Awake()
    {
        _inventory.Init();
    }

    public void SetQuickable(IQuickable quickable, int index)
    {
        if (quickable == null)
        {
            return;
        }

        if (_inventory.Items[index] == quickable)
        {
            return;
        }

        if (_inventory.SetItem(quickable, index))
        {
            if (quickable is Item item)
            {
                item.Destroyed += OnItemDestroyed;
            }

            InventoryChanged?.Invoke(quickable, index);
        }
    }

    public void RemoveQuickable(int index)
    {
        var quickable = _inventory.GetItem<IQuickable>(index);

        if (_inventory.RemoveItem(index))
        {
            if (quickable is Item item)
            {
                if (!_inventory.IsIncluded(quickable))
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

    public void SwapQuickable(int indexA, int indexB)
    {
        _inventory.SwapItem(indexA, indexB);
        InventoryChanged?.Invoke(_inventory.Items[indexA], indexA);
        InventoryChanged?.Invoke(_inventory.Items[indexB], indexB);
    }

    private void OnItemDestroyed(Item item)
    {
        var indexes = _inventory.GetItemAllIndex(item as IQuickable);
        foreach (var index in indexes)
        {
            RemoveQuickable(index);
        }
    }
}
