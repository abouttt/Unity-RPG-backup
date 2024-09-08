using UnityEngine;

public class UI_ArmorSlot : UI_EquipmentSlot
{
    [SerializeField]
    private ArmorType _armorType;

    protected override void Init()
    {
        base.Init();
        Refresh(null);
    }

    public void Refresh(ArmorItem armorItem)
    {
        if (armorItem != null)
        {
            SetObject(armorItem, armorItem.Data.ItemImage);
            ChangeBackgroundImage(true);
        }
        else
        {
            Clear();
        }
    }

    protected override void Clear()
    {
        base.Clear();
        ChangeBackgroundImage(false);
    }

    protected override bool CanDropItem(EquipmentItem equipmentItem)
    {
        if (equipmentItem is not ArmorItem armorItem)
        {
            return false;
        }

        if (armorItem.ArmorData.ArmorType != _armorType)
        {
            return false;
        }

        return true;
    }
}
