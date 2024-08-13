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

        return true;
    }

    public bool CanUse()
    {
        return true;
    }
}
