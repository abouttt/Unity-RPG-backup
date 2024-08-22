using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
        KeyInfoText,
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

    public void Setup(int bindingIndex)
    {
        Index = bindingIndex;
        GetText((int)Texts.KeyInfoText).text = Managers.Input.GetBindingPath("Quick", bindingIndex);
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
                SetObject(quickable, item.Data.ItemImage);

                if (item is IStackableItem stackable)
                {
                    stackable.CountChanged += RefreshCountText;
                }

                if (item.Data is ICooldownable cooldownable)
                {
                    Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).ConnectSystem(cooldownable.Cooldown);
                }
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
            if (item is IStackableItem stackable)
            {
                stackable.CountChanged -= RefreshCountText;
            }

            if (item.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).DeconnectSystem();
            }
        }

        base.Clear();
        GetText((int)Texts.CountText).gameObject.SetActive(false);
    }

    private void RefreshCountText()
    {
        if (ObjectRef is IStackableItem stackable)
        {
            GetText((int)Texts.CountText).gameObject.SetActive(true);
            GetText((int)Texts.CountText).text = stackable.Count.ToString();
        }
        else
        {
            GetText((int)Texts.CountText).gameObject.SetActive(false);
        }
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

        if (ObjectRef is IQuickable quickable)
        {
            quickable.UseQuick();
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
