using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponSlot : UI_EquipmentSlot
{
    enum Imagesss
    {
        InactiveImage = 3,
    }

    public WeaponItem WeaponItemRef => EquipmentItemRef as WeaponItem;

    [SerializeField]
    private bool _isRightHand;

    protected override void Init()
    {
        base.Init();
        BindImage(typeof(Imagesss));
        Clear();
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
                    SetImage(weaponItem.Data.ItemImage);
                    ChangeBackgroundImage(true);
                    EquipmentItemRef = weaponItem;
                }
            }
            else
            {
                if (handedType == HandedType.Two)
                {
                    if (EquipmentItemRef != null)
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
                    SetImage(weaponItem.Data.ItemImage);
                    GetImage((int)Images.SlotImage).color = Color.white;
                    ChangeBackgroundImage(true);
                    EquipmentItemRef = weaponItem;
                    _canDrag = true;
                }
            }
        }
        else
        {
            if (EquipmentItemRef != null)
            {
                if (equipmentInventory.IsEquipped(EquipmentItemRef.EquipmentData))
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
                        SetVirtualTwoHanded(equippedItem);
                        EquipmentItemRef = null;
                        return;
                    }
                }
            }

            Clear();
        }
    }

    private void Clear()
    {
        SetImage(null);
        ChangeBackgroundImage(false);
        GetImage((int)Imagesss.InactiveImage).gameObject.SetActive(false);
        EquipmentItemRef = null;
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

        if (!_isRightHand && EquipmentItemRef == null)
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
