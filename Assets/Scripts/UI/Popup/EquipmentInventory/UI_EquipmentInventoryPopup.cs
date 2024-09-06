using UnityEngine;

public class UI_EquipmentInventoryPopup : UI_Popup, ISystemConnectable<EquipmentInventory>
{
    enum Texts
    {
        HPAmountText,
        MPAmountText,
        SPAmountText,
        DamageAmountText,
        DefenseAmountText,
    }

    enum Buttons
    {
        CloseButton,
    }

    public EquipmentInventory EquipmentInventoryRef { get; private set; }

    private UI_ArmorSlot[] _armorSlots;
    private UI_WeaponSlot[] _weaponSlots;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        _armorSlots = GetComponentsInChildren<UI_ArmorSlot>();
        _weaponSlots = GetComponentsInChildren<UI_WeaponSlot>();

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_EquipmentInventoryPopup>);

        Managers.UI.Register(this);
    }

    public void ConnectSystem(EquipmentInventory equipmentInventory)
    {
        if (equipmentInventory == null)
        {
            return;
        }

        DeconnectSystem();

        EquipmentInventoryRef = equipmentInventory;
        equipmentInventory.InventoryChanged += RefreshSlot;
    }

    public void DeconnectSystem()
    {
        if (EquipmentInventoryRef != null)
        {
            EquipmentInventoryRef.InventoryChanged -= RefreshSlot;
            EquipmentInventoryRef = null;
        }
    }

    private void RefreshSlot(EquipmentItem equipmentItem, EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Armor:
                var armorItem = equipmentItem as ArmorItem;
                _armorSlots[(int)armorItem.ArmorData.ArmorType].Refresh(armorItem);
                break;
            case EquipmentType.Weapon:
                var weaponItem = equipmentItem as WeaponItem;
                foreach (var weaponSlot in _weaponSlots)
                {
                    weaponSlot.Refresh(weaponItem);
                }
                break;
        }
    }
}
