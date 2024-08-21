using UnityEngine;

public class EquipmentItem : Item, IUsableItem
{
    public EquipmentItemData EquipmentData => Data as EquipmentItemData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    { }

    public void Use(ItemInventory itemInventory, EquipmentInventory equipmentInventory)
    {
        if (!CanUse())
        {
            return;
        }

        var equippedItem = equipmentInventory.GetItem(EquipmentData.EquipmentType);
        if (equippedItem == this)
        {
            equipmentInventory.UnequipItem(EquipmentData.EquipmentType);
            itemInventory.AddItem(EquipmentData);
        }
        else
        {
            int index = itemInventory.GetItemIndex(this);

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
    }

    public bool CanUse()
    {
        return true;
    }
}
