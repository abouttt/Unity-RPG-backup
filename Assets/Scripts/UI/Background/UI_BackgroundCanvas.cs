using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, ISystemConnectable<Inventories>, IPointerDownHandler, IDropHandler
{
    public Inventories SystemRef { get; private set; }

    [SerializeField, Space(10), TextArea]
    private string DestroyItemText;

    protected override void Init()
    {
        Managers.UI.Register(this);
    }

    public void ConnectSystem(Inventories inventories)
    {
        SystemRef = inventories;
    }

    public void DeconnectSystem()
    {
        if (SystemRef != null)
        {
            SystemRef = null;
        }
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
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        var item = itemSlot.ObjectRef as Item;
        string text = $"[{item.Data.ItemName}] {DestroyItemText}";
        Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
        {
            SystemRef.ItemInventory.RemoveItem(itemSlot.Index);
        },
        text);
    }
}
