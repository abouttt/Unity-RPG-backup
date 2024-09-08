using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        QuantityText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; set; } = -1;

    protected override void Init()
    {
        base.Init();
        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));
        Refresh(null);
    }

    public void Refresh(Item item)
    {
        if (item != null)
        {
            if (ObjectRef == item)
            {
                return;
            }

            if (HasObject)
            {
                Clear();
            }

            SetObject(item, item.Data.ItemImage);

            if (item is IStackable stackable)
            {
                stackable.StackChanged += RefreshQuantityText;
                GetText((int)Texts.QuantityText).gameObject.SetActive(true);
                RefreshQuantityText(stackable);
            }

            if (item.Data is ICooldownable cooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).ConnectSystem(cooldownable.Cooldown);
            }
        }
        else
        {
            Clear();
        }
    }

    protected override void Clear()
    {
        if (ObjectRef is Item item)
        {
            if (item is IStackable stackable)
            {
                stackable.StackChanged -= RefreshQuantityText;
            }

            if (item.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).DeconnectSystem();
            }
        }

        base.Clear();
        GetText((int)Texts.QuantityText).gameObject.SetActive(false);
    }

    private void RefreshQuantityText(IStackable stackable)
    {
        GetText((int)Texts.QuantityText).text = stackable.Quantity.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_ItemInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp(eventData))
        {
            return;
        }

        if (ObjectRef is IUsable)
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.UseItem(Index);
        }
        else if (ObjectRef is EquipmentItem equipmentItem)
        {
            var itemInventory = Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef;
            var equipmentInventory = Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef;

            if (equipmentItem is WeaponItem weaponItem &&
                weaponItem.WeaponData.HandedType == HandedType.Left &&
                equipmentInventory.IsEquippedWeaponHandedTypeIs(HandedType.Two))
            {
                return;
            }

            if (equipmentInventory.IsEquipped(equipmentItem.EquipmentData))
            {
                var equippedItem = equipmentInventory.GetEquipment(equipmentItem.EquipmentData);
                itemInventory.SetItem(equippedItem.Data, Index);
            }
            else
            {
                itemInventory.RemoveItem(Index);
            }

            equipmentInventory.Equip(equipmentItem.EquipmentData);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().SetSlot(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().SetSlot(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(otherSlot as UI_ItemSlot);
                    break;
                case SlotType.Equipment:
                    OnDropEquipmentSlot(otherSlot as UI_EquipmentSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        var otherItem = otherItemSlot.ObjectRef as Item;

        if (!HasObject && otherItem is IStackable otherStackable && otherStackable.Quantity > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.SplitItem(otherItemSlot.Index, Index, splitPopup.Quantity);
            },
            $"[{otherItem.Data.ItemName}] {GuideSettings.Instance.ItemSpliteText}", 1, otherStackable.Quantity);
        }
        else
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.MoveItem(otherItemSlot.Index, Index);
        }
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot otherEquipmentSlot)
    {
        var otherEquipmentItem = otherEquipmentSlot.ObjectRef as EquipmentItem;

        if (ObjectRef is Item item)
        {
            if (item is not EquipmentItem equipmentItem)
            {
                return;
            }

            if (!IsSameDeepEquipmentType(equipmentItem.EquipmentData, otherEquipmentItem.EquipmentData))
            {
                return;
            }

            Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Equip(equipmentItem.EquipmentData);
        }
        else
        {
            Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Unequip(otherEquipmentItem.EquipmentData);
        }

        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.SetItem(otherEquipmentItem.Data, Index);
    }

    private bool IsSameDeepEquipmentType(EquipmentItemData a, EquipmentItemData b)
    {
        if (a.EquipmentType != b.EquipmentType)
        {
            return false;
        }

        if (a is ArmorItemData aArmorData && b is ArmorItemData bArmorData)
        {
            return aArmorData.ArmorType == bArmorData.ArmorType;
        }

        if (a is WeaponItemData aWeaponData && b is WeaponItemData bWeaponData)
        {
            if (aWeaponData.HandedType != HandedType.Left)
            {
                return bWeaponData.HandedType != HandedType.Left;
            }

            return aWeaponData.HandedType == bWeaponData.HandedType;
        }

        return false;
    }
}
