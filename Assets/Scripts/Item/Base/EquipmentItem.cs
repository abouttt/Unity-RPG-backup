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

        var equippedItem = s_inventoriesRef.EquipmentInventory.GetItem(EquipmentData.EquipmentType);
        if (equippedItem == this)
        {
            s_inventoriesRef.EquipmentInventory.UnequipItem(EquipmentData.EquipmentType);
            s_inventoriesRef.ItemInventory.AddItem(EquipmentData);
        }
        else
        {
            int index = s_inventoriesRef.ItemInventory.GetItemIndex(this);
            if (index == -1)
            {
                return false;
            }

            if (equippedItem != null)
            {
                s_inventoriesRef.ItemInventory.SetItem(equippedItem.EquipmentData, index);
            }
            else
            {
                s_inventoriesRef.ItemInventory.RemoveItem(index);
            }

            s_inventoriesRef.EquipmentInventory.EquipItem(EquipmentData);
        }

        return true;
    }

    public bool CanUse()
    {
        return true;
    }
}
