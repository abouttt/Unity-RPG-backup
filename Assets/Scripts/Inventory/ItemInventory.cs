using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public event Action<Item, int> InventoryChanged;

    public IReadOnlyList<Item> Items => _inventory.Items;

    [SerializeField]
    private Inventory<Item> _inventory;

    private void Awake()
    {
        _inventory.Init();
    }

    public int AddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null)
        {
            return -1;
        }

        if (quantity <= 0)
        {
            return -1;
        }

        var stackableData = itemData as StackableItemData;
        bool isStackable = stackableData != null;

        // 같은 아이템에 개수 더하기 시도
        if (isStackable)
        {
            for (int index = 0; index < _inventory.Capacity; index++)
            {
                if (quantity <= 0)
                {
                    break;
                }

                int sameItemIndex = FindSameItemIndex(index, stackableData);
                if (sameItemIndex != -1)
                {
                    var sameItem = _inventory.GetItem<StackableItem>(sameItemIndex);
                    if (!sameItem.IsMax)
                    {
                        quantity = sameItem.StackAndGetExcess(quantity);
                    }
                    index = sameItemIndex;
                }
                else
                {
                    break;
                }
            }
        }

        // 빈 공간에 아이템 추가 시도
        for (int index = 0; index < _inventory.Capacity; index++)
        {
            if (quantity <= 0)
            {
                break;
            }

            int emptyIndex = _inventory.FindEmptyIndex(index);
            if (emptyIndex != -1)
            {
                SetItem(itemData, emptyIndex, quantity);
                quantity = isStackable ? Mathf.Max(0, quantity - stackableData.MaxQuantity) : quantity - 1;
                index = emptyIndex;
            }
            else
            {
                break;
            }
        }

        return quantity;
    }

    public void RemoveItem(Item item)
    {
        int index = _inventory.GetItemIndex(item);
        RemoveItem(index);
    }

    public void RemoveItem(int index)
    {
        if (_inventory.RemoveItem(index))
        {
            InventoryChanged?.Invoke(null, index);
        }
    }

    public void SetItem(ItemData itemData, int index, int quantity = 1)
    {
        var newItem = itemData is StackableItemData stackableData
                    ? stackableData.CreateItem(quantity)
                    : itemData.CreateItem();
        if (_inventory.SetItem(newItem, index))
        {
            InventoryChanged?.Invoke(newItem, index);
        }
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!TryMergeItem(fromIndex, toIndex))
        {
            SwapItem(fromIndex, toIndex);
        }
    }

    public void SplitItem(int fromIndex, int toIndex, int quantity)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (quantity <= 0)
        {
            return;
        }

        if (!_inventory.HasItem(fromIndex) || _inventory.HasItem(toIndex))
        {
            return;
        }

        var fromItem = _inventory.GetItem<StackableItem>(fromIndex);
        int remaining = fromItem.Quantity - quantity;
        if (remaining <= 0)
        {
            SwapItem(fromIndex, toIndex);
        }
        else
        {
            fromItem.Quantity = remaining;
            SetItem(fromItem.StackableData, toIndex, quantity);
        }
    }

    public bool UseItem(int index)
    {
        if (!_inventory.HasItem(index))
        {
            return false;
        }

        if (_inventory.Items[index] is not IUsable usable)
        {
            return false;
        }

        bool succeeded = usable.Use();

        if (succeeded)
        {
            if (usable is StackableItem stackableItem && stackableItem.IsEmpty)
            {
                RemoveItem(index);
            }
        }

        return succeeded;
    }

    public T GetItem<T>(int index) where T : Item
    {
        return _inventory.GetItem<T>(index);
    }

    public int GetIndex(Item item)
    {
        return _inventory.GetItemIndex(item);
    }

    public int FindSameItemIndex(int startIndex, ItemData itemData)
    {
        if (itemData == null)
        {
            return -1;
        }

        return _inventory.FindIndex(startIndex, item => item != null && item.Data.Equals(itemData));
    }

    private bool TryMergeItem(int fromIndex, int toIndex)
    {
        var fromItem = _inventory.GetItem<StackableItem>(fromIndex);
        var toItem = _inventory.GetItem<StackableItem>(toIndex);

        if (fromItem == null || toItem == null)
        {
            return false;
        }

        if (!fromItem.Data.Equals(toItem.Data))
        {
            return false;
        }

        if (toItem.IsMax)
        {
            return false;
        }

        fromItem.Quantity = toItem.StackAndGetExcess(fromItem.Quantity);
        if (fromItem.IsEmpty)
        {
            RemoveItem(fromIndex);
        }

        return true;
    }

    private void SwapItem(int fromIndex, int toIndex)
    {
        _inventory.SwapItem(fromIndex, toIndex);
        InventoryChanged?.Invoke(_inventory.Items[fromIndex], fromIndex);
        InventoryChanged?.Invoke(_inventory.Items[toIndex], toIndex);
    }
}
