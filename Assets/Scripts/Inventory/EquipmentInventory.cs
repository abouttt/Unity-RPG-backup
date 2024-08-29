using System;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    public event Action<EquipmentItem, EquipmentType> InventoryChanged;

    private readonly Inventory<ArmorItem> _armorInventory = new();
    private readonly Inventory<WeaponItem> _weaponInventory = new();

    private void Awake()
    {
        _armorInventory.Init(Enum.GetValues(typeof(ArmorType)).Length);
        _weaponInventory.Init(Enum.GetValues(typeof(WeaponType)).Length);
    }

    public void EquipArmor(ArmorItemData armorItemData)
    {
        var armorType = armorItemData.ArmorType;
        UnequipArmor(armorType);

        var newArmorItem = armorItemData.CreateItem() as ArmorItem;
        _armorInventory.SetItem(newArmorItem, (int)armorType);
        InventoryChanged?.Invoke(newArmorItem, EquipmentType.Armor);
    }

    public void EquipWeapon(WeaponItemData weaponItemData)
    {
        var weaponType = weaponItemData.WeaponType;
        UnequipWeapon(weaponType);

        if (weaponType == WeaponType.OneHanded ||
            weaponType == WeaponType.Shield)
        {
            UnequipWeapon(WeaponType.TwoHanded);
        }
        else if (weaponType == WeaponType.TwoHanded)
        {
            UnequipWeapon(WeaponType.OneHanded);
            UnequipWeapon(WeaponType.Shield);
        }

        var newWeaponItem = weaponItemData.CreateItem() as WeaponItem;
        _weaponInventory.SetItem(newWeaponItem, (int)weaponType);
        InventoryChanged?.Invoke(newWeaponItem, EquipmentType.Weapon);
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

    public void UnequipWeapon(WeaponType weaponType)
    {
        if (!IsWeaponEquipped(weaponType))
        {
            return;
        }

        _weaponInventory.RemoveItem((int)weaponType);
        InventoryChanged?.Invoke(null, EquipmentType.Weapon);
    }

    public ArmorItem GetArmor(ArmorType armorType)
    {
        return _armorInventory.GetItem((int)armorType);
    }

    public WeaponItem GetWeapon(WeaponType weaponType)
    {
        return _weaponInventory.GetItem((int)weaponType);
    }

    public bool IsArmorEquipped(ArmorType armorType)
    {
        return _armorInventory.HasItem((int)armorType);
    }

    public bool IsWeaponEquipped(WeaponType weaponType)
    {
        return _weaponInventory.HasItem((int)weaponType);
    }
}
