using System.Collections.Generic;
using UnityEngine;

public class UI_ItemInventoryPopup : UI_Popup
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

    private readonly List<UI_ItemSlot> _slots = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RTs));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        Managers.UI.Register(this);
    }

    protected override void Start()
    {
        base.Start();

        Showed += () =>
        {
            PopupRT.SetParent(transform);
        };

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);
    }

    public void Setup(ItemInventory itemInventory)
    {
        ItemInventoryRef = itemInventory;
        itemInventory.InventoryChanged += (item, index) => _slots[index].Refresh(item);
        InitSlots(itemInventory.Items.Count, GetRT((int)RTs.ItemSlots));
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
}
