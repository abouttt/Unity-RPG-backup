using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour, IInventory
{
    public event Action<Item, int> InventoryChanged;

    public IReadOnlyList<Item> Items => _inventory.Items;

    [SerializeField]
    private Inventory<Item> _inventory;

    private void Awake()
    {
        _inventory.Init();
        Item.SetItemInventory(this);
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
        if (itemData == null)
        {
            return -1;
        }

        if (count <= 0)
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
                if (count <= 0)
                {
                    break;
                }

                int sameItemIndex = FindSameItemIndex(index, stackableData);
                if (sameItemIndex != -1)
                {
                    var sameItem = _inventory.GetItem<StackableItem>(sameItemIndex);
                    count = sameItem.IsMax ? count : sameItem.AddCountAndGetExcess(count);
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
            if (count <= 0)
            {
                break;
            }

            int emptyIndex = _inventory.FindEmptyIndex(index);
            if (emptyIndex != -1)
            {
                SetItem(itemData, emptyIndex, count);
                count = isStackable ? Mathf.Max(0, count - stackableData.MaxCount) : count - 1;
                index = emptyIndex;
            }
            else
            {
                break;
            }
        }

        return count;
    }

    public void RemoveItem(int index)
    {
        var item = _inventory.GetItem<Item>(index);
        if (_inventory.RemoveItem(index))
        {
            item.Destroy();
            InventoryChanged?.Invoke(null, index);
        }
    }

    public void RemoveItem(Item item)
    {
        int index = _inventory.GetItemIndex(item);
        if (_inventory.RemoveItem(index))
        {
            item.Destroy();
            InventoryChanged?.Invoke(null, index);
        }
    }

    public void SetItem(ItemData itemData, int index, int count = 1)
    {
        var newItem = itemData is StackableItemData stackableData
                    ? stackableData.CreateItem(count)
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

    public void SplitItem(int fromIndex, int toIndex, int count)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (count <= 0)
        {
            return;
        }

        if (_inventory.IsEmptyIndex(fromIndex) || !_inventory.IsEmptyIndex(toIndex))
        {
            return;
        }

        var fromItem = _inventory.GetItem<StackableItem>(fromIndex);
        int remainingCount = fromItem.Count - count;
        if (remainingCount <= 0)
        {
            SwapItem(fromIndex, toIndex);
        }
        else
        {
            fromItem.Count = remainingCount;
            SetItem(fromItem.StackableData, toIndex, count);
        }
    }

    public T GetItem<T>(int index) where T : Item
    {
        return _inventory.GetItem<T>(index);
    }

    public int GetItemIndex(Item item)
    {
        return _inventory.GetItemIndex(item);
    }

    public int FindSameItemIndex(int startIndex, ItemData itemData)
    {
        return _inventory.FindSameItemIndex(startIndex, item => item != null && item.Data.Equals(itemData));
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

        int excessCount = toItem.AddCountAndGetExcess(fromItem.Count);
        fromItem.Count = excessCount;
        if (fromItem.IsEmpty)
        {
            RemoveItem(fromItem);
        }

        return true;
    }

    private void SwapItem(int indexA, int indexB)
    {
        _inventory.SwapItem(indexA, indexB);
        InventoryChanged?.Invoke(_inventory.Items[indexA], indexA);
        InventoryChanged?.Invoke(_inventory.Items[indexB], indexB);
    }
}
