using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        Refresh(null);
    }

    public void Setup(int index)
    {
        Index = index;
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
                stackable.StackChanged += RefreshCountText;
            }

            if (item.Data is ICooldownable cooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
            }

            RefreshCountText();
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
                stackable.StackChanged -= RefreshCountText;
            }

            if (item.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
            }
        }

        base.Clear();
        GetText((int)Texts.CountText).gameObject.SetActive(false);
    }

    private void RefreshCountText()
    {
        if (ObjectRef is IStackable stackable && stackable.Count > 1)
        {
            GetText((int)Texts.CountText).gameObject.SetActive(true);
            GetText((int)Texts.CountText).text = stackable.Count.ToString();
        }
        else
        {
            GetText((int)Texts.CountText).gameObject.SetActive(false);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_ItemInventoryPopup>().SetTop();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // TODO : Tooltip On
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // TODO : Tooltip Off
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        if (ObjectRef is IUsable usable)
        {
            usable.Use();
        }
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
        var itemInventory = Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef;

        if (!HasObject && otherItem is IStackable otherStackable && otherStackable.Count > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
            {
                itemInventory.SplitItem(otherItemSlot.Index, Index, splitPopup.Count);
            },
            $"[{otherItem.Data.ItemName}] ������ ������", 1, otherStackable.Count);
        }
        else
        {
            itemInventory.MoveItem(otherItemSlot.Index, Index);
        }
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot otherEquipmentSlot)
    {
        var otherEquipmentItem = otherEquipmentSlot.ObjectRef as EquipmentItem;
        var itemInventory = Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef;
        var equipmentInventory = Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef;

        if (HasObject)
        {
            if (ObjectRef is not EquipmentItem equipmentItem)
            {
                return;
            }

            if (equipmentItem.EquipmentData.EquipmentType != otherEquipmentItem.EquipmentData.EquipmentType)
            {
                return;
            }

            if (equipmentItem is not IUsable usable)
            {
                return;
            }

            usable.Use();
        }
        else
        {
            equipmentInventory.UnequipItem(otherEquipmentSlot.EquipmentType);
            itemInventory.SetItem(otherEquipmentItem.Data, Index);
        }
    }
}
