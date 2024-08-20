using UnityEngine;

public class Inventories
{
    public readonly ItemInventory ItemInventory;
    public readonly EquipmentInventory EquipmentInventory;

    public Inventories(ItemInventory itemInventory, EquipmentInventory equipmentInventory)
    {
        ItemInventory = itemInventory;
        EquipmentInventory = equipmentInventory;
    }
}
