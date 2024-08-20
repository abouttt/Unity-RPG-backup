using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, ISystemConnectable<Inventories>, IPointerDownHandler, IDropHandler
{
    [SerializeField, Space(10), TextArea]
    private string DestroyItemText;

    private Inventories _inventoriesRef;

    protected override void Init()
    {
        Managers.UI.Register(this);
    }

    public void ConnectSystem(Inventories inventories)
    {
        _inventoriesRef = inventories;
    }

    public void DeconnectSystem()
    {
        _inventoriesRef = null;
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
        string text = $"[{item.Data.ItemName}] {DestroyItemText}";
        Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
        {
            _inventoriesRef.ItemInventory.RemoveItem(itemSlot.Index);
        },
        text);
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot equipmentSlot)
    {
        var equipmentItem = equipmentSlot.ObjectRef as EquipmentItem;
        _inventoriesRef.EquipmentInventory.UnequipItem(equipmentSlot.EquipmentType);
        _inventoriesRef.ItemInventory.AddItem(equipmentItem.Data);
    }
}
