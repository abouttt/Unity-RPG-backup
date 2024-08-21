using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/HPPotion", fileName = "Consumable_HPPotion")]
public class HPPotionData : ConsumableItemData
{
    [field: SerializeField]
    public float HealAmount { get; private set; }

    public override Item CreateItem()
    {
        return new HPPotion(this, 1);
    }

    public override Item CreateItem(int count)
    {
        return new HPPotion(this, count);
    }
}
