using System;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    public event Action<EquipmentItem, EquipmentType> InventoryChanged;

    [SerializeField, ReadOnly]
    private Inventory<ArmorItem> _armorInventory;

    [SerializeField, ReadOnly]
    private Inventory<WeaponItem> _weaponInventory;

    private void Awake()
    {
        _armorInventory.Init(Enum.GetValues(typeof(ArmorType)).Length);
        _weaponInventory.Init(2);   // Two hands
    }

    public void Equip(EquipmentItemData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        switch (equipmentData.EquipmentType)
        {
            case EquipmentType.Armor:
                EquipArmor(equipmentData as ArmorItemData);
                break;
            case EquipmentType.Weapon:
                EquipWeapon(equipmentData as WeaponItemData);
                break;
        }
    }

    public void EquipArmor(ArmorItemData armorData)
    {
        UnequipArmor(armorData.ArmorType);

        var newArmorItem = armorData.CreateItem() as ArmorItem;
        _armorInventory.SetItem(newArmorItem, (int)armorData.ArmorType);
        InventoryChanged?.Invoke(newArmorItem, EquipmentType.Armor);
    }

    public void EquipWeapon(WeaponItemData weaponData)
    {
        var handedType = TransformHandedType(weaponData.HandedType);
        UnequipWeapon(handedType);

        var newWeaponItem = weaponData.CreateItem() as WeaponItem;
        _weaponInventory.SetItem(newWeaponItem, (int)handedType);
        InventoryChanged?.Invoke(newWeaponItem, EquipmentType.Weapon);
    }

    public void Unequip(EquipmentItemData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        switch (equipmentData.EquipmentType)
        {
            case EquipmentType.Armor:
                UnequipArmor((equipmentData as ArmorItemData).ArmorType);
                break;
            case EquipmentType.Weapon:
                UnequipWeapon((equipmentData as WeaponItemData).HandedType);
                break;
        }
    }

    public void UnequipArmor(ArmorType armorType)
    {
        if (!IsArmorEquipped(armorType))
        {
            return;
        }

        _armorInventory.RemoveItem((int)armorType);
        InventoryChanged?.Invoke(null, EquipmentType.Armor);
    }

    public void UnequipWeapon(HandedType handedType)
    {
        handedType = TransformHandedType(handedType);
        if (!IsWeaponEquipped(handedType))
        {
            return;
        }

        _weaponInventory.RemoveItem((int)handedType);
        InventoryChanged?.Invoke(null, EquipmentType.Weapon);
    }

    public EquipmentItem GetEquipment(EquipmentItemData equipmentData)
    {
        return equipmentData.EquipmentType switch
        {
            EquipmentType.Armor => GetArmor((equipmentData as ArmorItemData).ArmorType),
            EquipmentType.Weapon => GetWeapon((equipmentData as WeaponItemData).HandedType),
            _ => null,
        };
    }

    public ArmorItem GetArmor(ArmorType armorType)
    {
        return _armorInventory[(int)armorType];
    }

    public WeaponItem GetWeapon(HandedType handedType)
    {
        handedType = TransformHandedType(handedType);
        return _weaponInventory[(int)handedType];
    }

    public bool IsEquipped(EquipmentItemData equipmentData)
    {
        return equipmentData.EquipmentType switch
        {
            EquipmentType.Armor => IsArmorEquipped((equipmentData as ArmorItemData).ArmorType),
            EquipmentType.Weapon => IsWeaponEquipped((equipmentData as WeaponItemData).HandedType),
            _ => false,
        };
    }

    public bool IsArmorEquipped(ArmorType armorType)
    {
        return _armorInventory.HasItem((int)armorType);
    }

    public bool IsWeaponEquipped(HandedType handedType)
    {
        handedType = TransformHandedType(handedType);
        return _weaponInventory.HasItem((int)handedType);
    }

    public bool IsEquippedWeaponHandedTypeIs(HandedType handedType)
    {
        var equippedItem = _weaponInventory[(int)TransformHandedType(handedType)];
        if (equippedItem == null)
        {
            return false;
        }

        return equippedItem.WeaponData.HandedType == handedType;
    }

    private HandedType TransformHandedType(HandedType handedType)
    {
        return handedType == HandedType.Two ? HandedType.Right : handedType;
    }
}
