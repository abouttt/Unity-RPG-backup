using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_EquipmentSlot : UI_BaseSlot, IDropHandler
{
    protected enum Imagess
    {
        BG = 2,
    }

    public EquipmentItem EquipmentItemRef { get; protected set; }

    [SerializeField]
    private Sprite _equipBackgroundImage;

    [SerializeField]
    private Sprite _unequipBackgroundImage;

    protected override void Init()
    {
        base.Init();
        BindImage(typeof(Imagess));
        GetImage((int)Imagess.BG).sprite = _unequipBackgroundImage;
    }

    protected void ChangeBackgroundImage(bool isEquipped)
    {
        GetImage((int)Imagess.BG).sprite = isEquipped ? _equipBackgroundImage : _unequipBackgroundImage;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (eventData != null && eventData.dragging)
        {
            return;
        }

        if (EquipmentItemRef != null)
        {
            Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().Show(EquipmentItemRef.Data);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Managers.UI.Get<UI_TopCanvas>().GetSubitem<UI_ItemTooltip>().Hide();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp(eventData))
        {
            return;
        }

        if (EquipmentItemRef == null)
        {
            return;
        }

        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.AddItem(EquipmentItemRef.EquipmentData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Unequip(EquipmentItemRef.EquipmentData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherItemSlot))
        {
            if (otherItemSlot.ItemRef is not EquipmentItem otherEquipmentItem)
            {
                return;
            }

            if (!CanDropItem(otherEquipmentItem))
            {
                return;
            }

            if (EquipmentItemRef != null)
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.SetItem(EquipmentItemRef.Data, otherItemSlot.Index);
            }
            else
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.RemoveItem(otherItemSlot.Index);
            }

            Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Equip(otherEquipmentItem.EquipmentData);
        }
    }

    protected abstract bool CanDropItem(EquipmentItem equipmentItem);
}
