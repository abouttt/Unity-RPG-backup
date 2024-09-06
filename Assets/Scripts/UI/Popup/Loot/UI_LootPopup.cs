using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_LootPopup : UI_Popup, ISystemConnectable<ItemInventory>
{
    enum RTs
    {
        LootSubitems,
    }

    enum Texts
    {
        LootAllText,
    }

    enum Buttons
    {
        CloseButton,
        LootAllButton,
    }

    [SerializeField]
    private float _trackingDistance;

    private ItemInventory _itemInventoryRef;
    private FieldItem _fieldItemRef;
    private InputAction _interact;
    private readonly Dictionary<UI_LootSubitem, ItemData> _subitems = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RTs));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        _interact = Managers.Input.GetAction("Interact");

        GetText((int)Texts.LootAllText).text = $"[{Managers.Input.GetBindingPath("Interact")}] ¸ðµÎ È¹µæ";
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_LootPopup>);
        GetButton((int)Buttons.LootAllButton).onClick.AddListener(AddAllItemToItemInventory);

        Showed += () => _interact.performed += AddAllItemToItemInventoryInputAction;

        Closed += () =>
        {
            Clear();
            _interact.performed -= AddAllItemToItemInventoryInputAction;
        };

        Managers.UI.Register(this);
    }

    private void Update()
    {
        if (_fieldItemRef == null)
        {
            Managers.UI.Close<UI_LootPopup>();
            return;
        }

        if (!_fieldItemRef.gameObject.activeSelf)
        {
            Managers.UI.Close<UI_LootPopup>();
            return;
        }

        TrackingFieldItem();
    }

    public void ConnectSystem(ItemInventory itemInventory)
    {
        if (itemInventory == null)
        {
            return;
        }

        _itemInventoryRef = itemInventory;
    }

    public void DeconnectSystem()
    {
        _itemInventoryRef = null;
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        _fieldItemRef = fieldItem;

        foreach (var kvp in _fieldItemRef.Items)
        {
            if (kvp.Key is StackableItemData stackableData)
            {
                int quantity = kvp.Value;
                while (quantity > 0)
                {
                    CreateSubitem(kvp.Key, Mathf.Clamp(quantity, quantity, stackableData.MaxQuantity));
                    quantity -= stackableData.MaxQuantity;
                }
            }
            else
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    CreateSubitem(kvp.Key, 1);
                }
            }
        }
    }

    public void AddItemToItemInventory(UI_LootSubitem subitem)
    {
        _fieldItemRef.RemoveItem(subitem.ItemDataRef, subitem.Quantity);
        int quantity = _itemInventoryRef.AddItem(subitem.ItemDataRef, subitem.Quantity);
        if (quantity > 0)
        {
            subitem.SetItemData(subitem.ItemDataRef, quantity);
        }
        else
        {
            RemoveSubitem(subitem);
        }
    }

    private void CreateSubitem(ItemData itemData, int count)
    {
        var subitem = Managers.Resource.Instantiate<UI_LootSubitem>("UI_LootSubitem.prefab", GetRT((int)RTs.LootSubitems), true);
        subitem.SetItemData(itemData, count);
        _subitems.Add(subitem, itemData);
    }

    private void RemoveSubitem(UI_LootSubitem subitem)
    {
        _subitems.Remove(subitem);
        Managers.Resource.Destroy(subitem.gameObject);
        if (_subitems.Count == 0)
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void TrackingFieldItem()
    {
        if (_trackingDistance < Vector3.Distance(_itemInventoryRef.gameObject.transform.position, _fieldItemRef.transform.position))
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void AddAllItemToItemInventory()
    {
        for (int i = _subitems.Count - 1; i >= 0; i--)
        {
            AddItemToItemInventory(_subitems.ElementAt(i).Key);
        }
    }

    private void Clear()
    {
        foreach (var kvp in _subitems)
        {
            Managers.Resource.Destroy(kvp.Key.gameObject);
        }

        _subitems.Clear();

        if (_fieldItemRef != null)
        {
            _fieldItemRef.StopInteract();
            _fieldItemRef = null;
        }
    }

    private void AddAllItemToItemInventoryInputAction(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            AddAllItemToItemInventory();
        }
    }
}
