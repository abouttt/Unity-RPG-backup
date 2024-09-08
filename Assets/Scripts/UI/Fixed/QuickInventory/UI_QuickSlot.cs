using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        QuantityText,
        KeyInfoText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            GetText((int)Texts.KeyInfoText).text = Managers.Input.GetBindingPath("Quick", _index);
        }
    }

    private int _index;

    protected override void Init()
    {
        base.Init();
        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));
        Refresh(null);
    }

    public void Refresh(IQuickable quickable)
    {
        if (quickable != null)
        {
            if (ObjectRef == quickable)
            {
                return;
            }

            if (HasObject)
            {
                Clear();
            }

            if (quickable is Item item)
            {
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

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp(eventData))
        {
            return;
        }

        if (ObjectRef is Item item)
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.UseItem(item);
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
                case SlotType.Quick:
                    OnDropQuickSlot(otherSlot as UI_QuickSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        if (otherItemSlot.ObjectRef is not IQuickable quickable)
        {
            return;
        }

        if (ObjectRef == quickable)
        {
            return;
        }

        Managers.UI.Get<UI_QuickInventoryFixed>().QuickInventoryRef.SetQuickable(quickable, Index);
    }

    private void OnDropQuickSlot(UI_QuickSlot otherQuickSlot)
    {
        Managers.UI.Get<UI_QuickInventoryFixed>().QuickInventoryRef.SwapQuickable(otherQuickSlot.Index, Index);
    }
}
