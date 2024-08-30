using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_BaseSlot, IDropHandler, IPointerUpHandler
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
    public Item ItemRef { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        Refresh(null);
    }

    public void Refresh(Item item)
    {
        ClearItemRef();

        if (item != null)
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

            ItemRef = item;
        }
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

        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.UseItem(Index);
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
        if (ItemRef == null && otherItemSlot.ItemRef is IStackable otherStackable && otherStackable.Quantity > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.SplitItem(otherItemSlot.Index, Index, splitPopup.Quantity);
            },
            $"[{otherItemSlot.ItemRef.Data.ItemName}] {GuideSettings.Instance.ItemSpliteText}", 1, otherStackable.Quantity);
        }
        else
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.MoveItem(otherItemSlot.Index, Index);
        }
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot otherEquipmentSlot)
    {

    }

    private void ClearItemRef()
    {
        if (ItemRef != null)
        {
            if (ItemRef is IStackable stackable)
            {
                stackable.StackChanged -= RefreshQuantityText;
            }

            if (ItemRef.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).DeconnectSystem();
            }

            ItemRef = null;
        }

        SetImage(null);
        GetText((int)Texts.QuantityText).gameObject.SetActive(false);
    }
}
