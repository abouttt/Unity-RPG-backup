using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_EquipmentSlot : UI_BaseSlot, IDropHandler
{
    protected enum Imagess
    {
        BG = 2,
    }

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

        var equipmentItem = ObjectRef as EquipmentItem;
        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.AddItem(equipmentItem.EquipmentData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Unequip(equipmentItem.EquipmentData);
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

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherItemSlot))
        {
            if (otherItemSlot.ObjectRef is not EquipmentItem otherEquipmentItem)
            {
                return;
            }

            if (!CanDropItem(otherEquipmentItem))
            {
                return;
            }

            if (ObjectRef is EquipmentItem equipmentItem)
            {
                Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.SetItem(equipmentItem.Data, otherItemSlot.Index);
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
