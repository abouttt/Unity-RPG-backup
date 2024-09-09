using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ItemTooltip : UI_TopSubitem
{
    enum RTs
    {
        Tooltip,
    }

    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemDescText,
    }

    [Space(10)]
    [SerializeField]
    private Color _commonColor = Color.white;

    [SerializeField]
    private Color _uncommonColor = Color.white;

    [SerializeField]
    private Color _rareColor = Color.white;

    [SerializeField]
    private Color _epicColor = Color.white;

    [SerializeField]
    private Color _legendaryColor = Color.white;

    private ItemData _itemDataRef;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();
        BindRT(typeof(RTs));
        BindText(typeof(Texts));
    }

    private void LateUpdate()
    {
        SetPosition(Mouse.current.position.ReadValue());
    }

    public void Show(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        gameObject.SetActive(true);
        RefreshItemData(itemData);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void RefreshItemData(ItemData itemData)
    {
        if (_itemDataRef != null && _itemDataRef.Equals(itemData))
        {
            return;
        }

        _itemDataRef = itemData;
        GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        SetItemRarityColor(itemData.ItemRarity);
        SetItemType(itemData.ItemType);
        SetDescription(itemData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetRT((int)RTs.Tooltip));
    }

    private void SetItemRarityColor(ItemRarity itemRarity)
    {
        GetText((int)Texts.ItemNameText).color = itemRarity switch
        {
            ItemRarity.Common => _commonColor,
            ItemRarity.Uncommon => _uncommonColor,
            ItemRarity.Rare => _rareColor,
            ItemRarity.Epic => _epicColor,
            ItemRarity.Legendary => _legendaryColor,
            _ => Color.red,
        };
    }

    private void SetItemType(ItemType itemType)
    {
        GetText((int)Texts.ItemTypeText).text = itemType switch
        {
            ItemType.Equipment => "[장비 아이템]",
            ItemType.Consumable => "[소비 아이템]",
            ItemType.Miscellaneous => "[기타 아이템]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        _sb.Clear();

        if (itemData is ILevelRequirement levelRequirement)
        {
            _sb.Append($"제한 레벨 : {levelRequirement.RequiredLevel}\n");
        }

        if (itemData is EquipmentItemData equipmentData)
        {
            // TODO : 스탯
        }
        else if (itemData is ConsumableItemData consumableData)
        {
            _sb.Append($"소비 개수 : {consumableData.ConsumptionQuantity}\n");
        }

        if (_sb.Length > 0)
        {
            _sb.Append("\n");
        }

        if (!string.IsNullOrEmpty(itemData.Description))
        {
            _sb.Append($"{itemData.Description}\n\n");
        }

        GetText((int)Texts.ItemDescText).text = _sb.ToString();
    }

    private void SetPosition(Vector3 position)
    {
        var rt = GetRT((int)RTs.Tooltip);
        rt.position = new Vector3()
        {
            x = position.x + (rt.rect.width * 0.5f),
            y = position.y + (rt.rect.height * 0.5f)
        };
    }
}
