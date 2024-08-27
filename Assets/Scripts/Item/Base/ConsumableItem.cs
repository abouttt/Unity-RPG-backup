using UnityEngine;

public abstract class ConsumableItem : StackableItem, IUsableItem, IQuickable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int count)
        : base(data, count)
    { }

    public void Use()
    {
        if (!CanUse())
        {
            return;
        }

        OnUse();

        Count -= ConsumableData.RequiredCount;
        if (IsEmpty)
        {
            ItemInventoryRef.RemoveItem(this);
        }

        ConsumableData.Cooldown.Start();
    }

    public bool CanUse()
    {
        if (Count < ConsumableData.RequiredCount)
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
