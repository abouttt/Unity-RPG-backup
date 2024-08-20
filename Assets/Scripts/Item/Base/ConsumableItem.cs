using UnityEngine;

public abstract class ConsumableItem : StackableItem, IUsable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int count)
        : base(data, count)
    { }

    public virtual bool Use()
    {
        if (!CanUse())
        {
            return false;
        }

        Count -= ConsumableData.RequiredCount;
        if (IsEmpty)
        {
            Managers.Inventory.Get<ItemInventory>().RemoveItem(this);
        }

        Managers.Cooldown.AddCooldown(ConsumableData.Cooldown);

        return true;
    }

    public bool CanUse()
    {
        return true;
    }
}
