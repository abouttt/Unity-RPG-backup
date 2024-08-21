using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, IPointerDownHandler, IDropHandler
{
    [SerializeField, Space(10), TextArea]
    private string _itemDestroyText;

    protected override void Init()
    {
        Managers.UI.Register(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Managers.Input.CursorLocked = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var slot))
        {
            switch (slot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(slot as UI_ItemSlot);
                    break;
                case SlotType.Equipment:
                    OnDropEquipmentSlot(slot as UI_EquipmentSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        var item = itemSlot.ObjectRef as Item;
        string text = $"[{item.Data.ItemName}] {_itemDestroyText}";
        Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.RemoveItem(itemSlot.Index);
        },
        text);
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot equipmentSlot)
    {
        var equipmentItem = equipmentSlot.ObjectRef as EquipmentItem;
        Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.UnequipItem(equipmentSlot.EquipmentType);
        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.AddItem(equipmentItem.Data);
    }
}
