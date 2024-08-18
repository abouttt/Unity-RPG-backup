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

    public ItemInventory SystemRef { get; private set; }

    [SerializeField]
    private float _trackingDistance;

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
        SystemRef = itemInventory;
    }

    public void DeconnectSystem()
    {
        if (SystemRef != null)
        {
            SystemRef = null;
        }
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        _fieldItemRef = fieldItem;

        foreach (var kvp in _fieldItemRef.Items)
        {
            if (kvp.Key is StackableItemData stackableData)
            {
                int count = kvp.Value;
                while (count > 0)
                {
                    CreateLootSubitem(kvp.Key, Mathf.Clamp(count, count, stackableData.MaxCount));
                    count -= stackableData.MaxCount;
                }
            }
            else
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    CreateLootSubitem(kvp.Key, 1);
                }
            }
        }
    }

    public void AddItemToItemInventory(UI_LootSubitem lootSubitem)
    {
        _fieldItemRef.RemoveItem(lootSubitem.ItemDataRef, lootSubitem.Count);
        int count = SystemRef.AddItem(lootSubitem.ItemDataRef, lootSubitem.Count);
        if (count > 0)
        {
            lootSubitem.SetItemData(lootSubitem.ItemDataRef, count);
        }
        else
        {
            RemoveLootSubitem(lootSubitem);
        }
    }

    private void CreateLootSubitem(ItemData itemData, int count)
    {
        var lootSubitem = Managers.Resource.Instantiate<UI_LootSubitem>("UI_LootSubitem.prefab", GetRT((int)RTs.LootSubitems), true);
        lootSubitem.SetItemData(itemData, count);
        _subitems.Add(lootSubitem, itemData);
    }

    private void RemoveLootSubitem(UI_LootSubitem lootSubitem)
    {
        _subitems.Remove(lootSubitem);
        Managers.Resource.Destroy(lootSubitem.gameObject);
        if (_subitems.Count == 0)
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void TrackingFieldItem()
    {
        if (_trackingDistance < Vector3.Distance(SystemRef.gameObject.transform.position, _fieldItemRef.transform.position))
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

    private void OnDestroy()
    {
        DeconnectSystem();
    }
}
