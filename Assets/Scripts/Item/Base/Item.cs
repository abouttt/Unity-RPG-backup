using System;
using UnityEngine;

public class Item : IItem
{
    public event Action<Item> Destroyed;

    public ItemData Data { get; private set; }

    protected static ItemInventory ItemInventoryRef { get; private set; }
    protected static EquipmentInventory EquipmentInventoryRef { get; private set; }

    public Item(ItemData itemData)
    {
        Data = itemData;
    }

    public static void SetItemInventory(ItemInventory itemInventory)
    {
        ItemInventoryRef = itemInventory;
    }

    public static void SetEquipmentInventory(EquipmentInventory equipmentInventory)
    {
        EquipmentInventoryRef = equipmentInventory;
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
        Destroyed = null;
    }
}
