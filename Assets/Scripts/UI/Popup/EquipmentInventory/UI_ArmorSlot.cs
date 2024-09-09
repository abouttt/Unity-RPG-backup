using UnityEngine;

public class UI_ArmorSlot : UI_EquipmentSlot
{
    public ArmorItem ArmorItemRef => EquipmentItemRef as ArmorItem;

    [SerializeField]
    private ArmorType _armorType;

    protected override void Init()
    {
        base.Init();
        Clear();
    }

    public void Refresh(ArmorItem armorItem)
    {
        Clear();

        if (armorItem != null)
        {
            SetImage(armorItem.Data.ItemImage);
            ChangeBackgroundImage(true);
            EquipmentItemRef = armorItem;
        }
    }

    public void Clear()
    {
        SetImage(null);
        ChangeBackgroundImage(false);
        EquipmentItemRef = null;
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
