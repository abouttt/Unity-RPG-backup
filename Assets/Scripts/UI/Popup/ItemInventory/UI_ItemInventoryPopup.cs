using System.Collections.Generic;
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

    public ItemInventory SystemRef { get; private set; }

    private readonly List<UI_ItemSlot> _slots = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RTs));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        Showed += () =>
        {
            PopupRT.SetParent(transform);
        };

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);

        Managers.UI.Register(this);
    }

    public void ConnectSystem(ItemInventory itemInventory)
    {
        SystemRef = itemInventory;
        itemInventory.InventoryChanged += RefreshSlot;
        InitSlots(itemInventory.Items.Count, GetRT((int)RTs.ItemSlots));
    }

    public void DeconnectSystem()
    {
        if (SystemRef == null)
        {
            return;
        }

        SystemRef.InventoryChanged -= RefreshSlot;
        SystemRef = null;
    }

    private void RefreshSlot(Item item, int index)
    {
        _slots[index].Refresh(item);
    }

    private void InitSlots(int count, Transform parent)
    {
        for (int index = 0; index < count; index++)
        {
            var itemSlot = Managers.Resource.Instantiate<UI_ItemSlot>("UI_ItemSlot.prefab");
            itemSlot.Setup(index);
            itemSlot.transform.SetParent(parent);
            _slots.Add(itemSlot);
        }
    }

    private void OnDestroy()
    {
        DeconnectSystem();
    }
}
