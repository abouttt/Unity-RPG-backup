using System.Collections.Generic;
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

    private readonly Dictionary<EquipmentType, UI_EquipmentSlot> _slots = new();

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        InitSlots();
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_EquipmentInventoryPopup>);

        Managers.UI.Register(this);
    }

    public void ConnectSystem(EquipmentInventory equipmentInventory)
    {
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
        _slots[equipmentType].Refresh(equipmentItem);
    }

    private void InitSlots()
    {
        var equipmentSlots = GetComponentsInChildren<UI_EquipmentSlot>();
        foreach (var equipmentSlot in equipmentSlots)
        {
            _slots.Add(equipmentSlot.EquipmentType, equipmentSlot);
        }
    }
}
