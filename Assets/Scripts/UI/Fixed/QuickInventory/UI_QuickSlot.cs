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

    public IQuickable QuickableRef { get; private set; }

    private int _index;

    protected override void Init()
    {
        base.Init();
        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));
        Clear();
    }

    public void Refresh(IQuickable quickable)
    {
        Clear();

        if (quickable == null)
        {
            return;
        }

        if (quickable is Item item)
        {
            SetImage(item.Data.ItemImage);

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

        QuickableRef = quickable;
    }

    private void RefreshQuantityText(IStackable stackable)
    {
        GetText((int)Texts.QuantityText).text = stackable.Quantity.ToString();
    }

    private void Clear()
    {
        if (QuickableRef != null)
        {
            if (QuickableRef is Item item)
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

            QuickableRef = null;
        }

        SetImage(null);
        GetText((int)Texts.QuantityText).gameObject.SetActive(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (eventData != null && eventData.dragging)
        {
            return;
        }

        if (QuickableRef is Item item)
        {
            Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().Show(item.Data);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().Hide();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp(eventData))
        {
            return;
        }

        if (QuickableRef == null)
        {
            return;
        }

        if (QuickableRef is Item item)
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.UseItem(item);
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
                case SlotType.Quick:
                    OnDropQuickSlot(otherSlot as UI_QuickSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        if (otherItemSlot.ItemRef is not IQuickable quickable)
        {
            return;
        }

        if (QuickableRef == quickable)
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