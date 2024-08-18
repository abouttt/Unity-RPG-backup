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
        CountText,
    }

    enum Buttons
    {
        LootButton,
    }

    public ItemData ItemDataRef { get; private set; }
    public int Count { get; private set; }

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

    public void SetItemData(ItemData itemData, int count)
    {
        if (ItemDataRef == null || !ItemDataRef.Equals(itemData))
        {
            ItemDataRef = itemData;
            GetImage((int)Images.ItemImage).sprite = itemData.ItemImage;
            GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        }

        Count = count;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        GetText((int)Texts.CountText).gameObject.SetActive(Count > 1);
        GetText((int)Texts.CountText).text = $"{Count}";
    }
}
