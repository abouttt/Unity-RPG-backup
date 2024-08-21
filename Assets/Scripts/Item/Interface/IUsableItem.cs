using UnityEngine;

public interface IUsableItem
{
    void Use(ItemInventory itemInventory, EquipmentInventory equipmentInventory);
    bool CanUse();
}
