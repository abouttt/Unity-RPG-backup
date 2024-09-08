using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponSlot : UI_EquipmentSlot
{
    enum Imagesss
    {
        InactiveImage = 3,
    }

    [SerializeField]
    private bool _isRightHand;

    protected override void Init()
    {
        base.Init();
        BindImage(typeof(Imagesss));
        Refresh(null);
    }

    public void Refresh(WeaponItem weaponItem)
    {
        var equipmentInventory = Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef;

        if (weaponItem != null)
        {
            var handedType = weaponItem.WeaponData.HandedType;

            if (_isRightHand)
            {
                if (handedType != HandedType.Left)
                {
                    SetObject(weaponItem, weaponItem.Data.ItemImage);
                    ChangeBackgroundImage(true);
                }
            }
            else
            {
                if (handedType == HandedType.Two)
                {
                    if (ObjectRef != null)
                    {
                        GetImage((int)Imagesss.InactiveImage).gameObject.SetActive(true);
                    }
                    else
                    {
                        SetVirtualTwoHanded(weaponItem);
                    }
                }
                else if (handedType == HandedType.Left)
                {
                    SetObject(weaponItem, weaponItem.Data.ItemImage);
                    GetImage((int)Images.SlotImage).color = Color.white;
                    ChangeBackgroundImage(true);
                    _canDrag = true;
                }
            }
        }
        else
        {
            if (ObjectRef is EquipmentItem equipmentItem)
            {
                if (equipmentInventory.IsEquipped(equipmentItem.EquipmentData))
                {
                    if (!_isRightHand)
                    {
                        GetImage((int)Imagesss.InactiveImage).gameObject.SetActive(false);
                    }
                    return;
                }
                else if (!_isRightHand)
                {
                    if (equipmentInventory.IsEquippedWeaponHandedTypeIs(HandedType.Two))
                    {
                        var equippedItem = equipmentInventory.GetWeapon(HandedType.Two);
                        base.Clear();
                        SetVirtualTwoHanded(equippedItem);
                        return;
                    }
                }
            }

            Clear();
        }
    }

    protected override void Clear()
    {
        base.Clear();
        ChangeBackgroundImage(false);
        GetImage((int)Imagesss.InactiveImage).gameObject.SetActive(false);
    }

    private void SetVirtualTwoHanded(WeaponItem twoHandedItem)
    {
        SetImage(twoHandedItem.Data.ItemImage);
        GetImage((int)Images.SlotImage).color = new Color(1f, 1f, 1f, 0.3f);
        GetImage((int)Imagesss.InactiveImage).gameObject.SetActive(false);
        ChangeBackgroundImage(true);
        _canDrag = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (!_isRightHand && !HasObject)
        {
            GetImage((int)Images.SlotImage).color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    protected override bool CanDropItem(EquipmentItem equipmentItem)
    {
        if (equipmentItem is not WeaponItem weaponItem)
        {
            return false;
        }

        if (weaponItem.WeaponData.HandedType == HandedType.Left)
        {
            if (Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.IsEquippedWeaponHandedTypeIs(HandedType.Two))
            {
                return false;
            }
        }

        return (weaponItem.WeaponData.HandedType != HandedType.Left) == _isRightHand;
    }
}
