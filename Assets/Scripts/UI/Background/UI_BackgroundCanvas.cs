using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, IPointerDownHandler, IDropHandler
{
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
                case SlotType.Quick:
                    OnDropQuickSlot(slot as UI_QuickSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        string text = $"[{itemSlot.ItemRef.Data.ItemName}] {GuideSettings.Instance.DestroyText}";
        Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.RemoveItem(itemSlot.Index);
        },
        text);
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot equipmentSlot)
    {
        var equipmentData = equipmentSlot.EquipmentItemRef.EquipmentData;
        Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef.AddItem(equipmentData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef.Unequip(equipmentData);
    }

    private void OnDropQuickSlot(UI_QuickSlot quickSlot)
    {
        Managers.UI.Get<UI_QuickInventoryFixed>().QuickInventoryRef.RemoveQuickable(quickSlot.Index);
    }
}
