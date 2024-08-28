using UnityEngine;

public abstract class ConsumableItem : StackableItem, IUsable, IQuickable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int quantity)
        : base(data, quantity)
    { }

    public bool Use()
    {
        if (!CanUse())
        {
            return false;
        }

        Quantity -= ConsumableData.ConsumptionQuantity;
        OnUse();

        return true;
    }

    public bool CanUse()
    {
        if (Quantity < ConsumableData.ConsumptionQuantity)
        {
            return false;
        }

        if (ConsumableData.Cooldown.RemainingTime > 0f)
        {
            return false;
        }

        return true;
    }

    public void UseQuick()
    {
        Use();
    }

    protected abstract void OnUse();
}
