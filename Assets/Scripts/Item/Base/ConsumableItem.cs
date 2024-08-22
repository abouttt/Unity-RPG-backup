using UnityEngine;

public abstract class ConsumableItem : StackableItem, IUsableItem, IQuickable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int count)
        : base(data, count)
    { }

    public void Use(ItemInventory itemInventory, EquipmentInventory equipmentInventory)
    {
        if (!CanUse())
        {
            return;
        }

        OnUse();

        Count -= ConsumableData.RequiredCount;
        if (IsEmpty)
        {
            itemInventory.RemoveItem(this);
        }

        Managers.Cooldown.AddCooldown(ConsumableData.Cooldown);
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
        Use(Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef, null);
    }

    protected abstract void OnUse();
}
