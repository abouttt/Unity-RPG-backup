using UnityEngine;

public class EquipmentItem : Item, IUsableItem
{
    public EquipmentItemData EquipmentData => Data as EquipmentItemData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    { }

    public void Use()
    {
        if (!CanUse())
        {
            return;
        }

        var equippedItem = EquipmentInventoryRef.GetItem(EquipmentData.EquipmentType);
        if (equippedItem == this)
        {
            EquipmentInventoryRef.UnequipItem(EquipmentData.EquipmentType);
            ItemInventoryRef.AddItem(EquipmentData);
        }
        else
        {
            int index = ItemInventoryRef.GetItemIndex(this);

            if (equippedItem != null)
            {
                ItemInventoryRef.SetItem(equippedItem.EquipmentData, index);
            }
            else
            {
                ItemInventoryRef.RemoveItem(index);
            }

            EquipmentInventoryRef.EquipItem(EquipmentData);
        }
    }

    public bool CanUse()
    {
        return true;
    }
}
