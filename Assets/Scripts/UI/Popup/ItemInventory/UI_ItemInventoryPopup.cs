using UnityEngine;

public class UI_ItemInventoryPopup : UI_Popup, ISystemConnectable<ItemInventory>
{
    enum RTs
    {
        ItemSlots,
    }

    enum Texts
    {
        GoldText
    }

    enum Buttons
    {
        CloseButton,
    }

    public ItemInventory ItemInventoryRef { get; private set; }

    private UI_ItemSlot[] _slots;

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RTs));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);

        Showed += () => PopupRT.SetParent(transform);

        Managers.UI.Register(this);
    }

    public void ConnectSystem(ItemInventory itemInventory)
    {
        ItemInventoryRef = itemInventory;
        itemInventory.InventoryChanged += RefreshSlot;
        CreateSlots(itemInventory.Capacity, GetRT((int)RTs.ItemSlots));
    }

    public void DeconnectSystem()
    {
        if (ItemInventoryRef != null)
        {
            ItemInventoryRef.InventoryChanged -= RefreshSlot;
            ItemInventoryRef = null;
        }
    }

    private void RefreshSlot(Item item, int index)
    {
        _slots[index].Refresh(item);
    }

    private void CreateSlots(int count, Transform parent)
    {
        for (int index = 0; index < count; index++)
        {
            var itemSlot = Managers.Resource.Instantiate<UI_ItemSlot>("UI_ItemSlot.prefab");
            itemSlot.Index = index;
            itemSlot.transform.SetParent(parent);
        }

        _slots = parent.GetComponentsInChildren<UI_ItemSlot>();
    }
}
