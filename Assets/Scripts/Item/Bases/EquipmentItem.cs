using UnityEngine;

public abstract class EquipmentItem : Item, IUsable
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

        return true;
    }

    public bool CanUse()
    {
        return true;
    }
}
