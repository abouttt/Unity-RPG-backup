using UnityEngine;

public class UI_LootSubitem : UI_Base
{
    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemNameText,
        QuantityText,
    }

    enum Buttons
    {
        LootButton,
    }

    public ItemData ItemDataRef { get; private set; }
    public int Quantity { get; private set; }

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.LootButton).onClick.AddListener(() =>
        {
            var lootPopup = Managers.UI.Get<UI_LootPopup>();
            lootPopup.SetTop();
            lootPopup.AddItemToItemInventory(this);
        });
    }

    public void SetItemData(ItemData itemData, int quantity)
    {
        if (ItemDataRef == null || !ItemDataRef.Equals(itemData))
        {
            ItemDataRef = itemData;
            GetImage((int)Images.ItemImage).sprite = itemData.ItemImage;
            GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        }

        Quantity = quantity;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        var quantityText = GetText((int)Texts.QuantityText);
        quantityText.gameObject.SetActive(Quantity > 1);
        quantityText.text = Quantity.ToString();
    }
}
