using UnityEngine;

public class EquipmentItem : Item, IUsable
{
    public EquipmentItemData EquipmentData => Data as EquipmentItemData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    { }

    public bool Use()
    {
        if (!CanUse())
        {
            return false;
        }

        var itemInventory = Managers.Inventory.Get<ItemInventory>();
        var equipmentInventory = Managers.Inventory.Get<EquipmentInventory>();

        var equippedItem = equipmentInventory.GetItem(EquipmentData.EquipmentType);
        if (equippedItem == this)
        {
            equipmentInventory.UnequipItem(EquipmentData.EquipmentType);
            itemInventory.AddItem(EquipmentData);
        }
        else
        {
            int index = itemInventory.GetItemIndex(this);
            if (index == -1)
            {
                return false;
            }

            if (equippedItem != null)
            {
                itemInventory.SetItem(equippedItem.EquipmentData, index);
            }
            else
            {
                itemInventory.RemoveItem(index);
            }

            equipmentInventory.EquipItem(EquipmentData);
        }

        return true;
    }

    public bool CanUse()
    {
        return true;
    }
}
