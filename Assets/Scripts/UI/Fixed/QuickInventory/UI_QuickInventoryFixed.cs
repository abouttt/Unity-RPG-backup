using UnityEngine;

public class UI_QuickInventoryFixed : UI_Base, ISystemConnectable<QuickInventory>
{
    enum RTs
    {
        QuickSlots,
    }

    public QuickInventory QuickInventoryRef { get; private set; }

    private UI_QuickSlot[] _slots;

    protected override void Init()
    {
        BindRT(typeof(RTs));
        Managers.UI.Register(this);
    }

    public void ConnectSystem(QuickInventory quickInventory)
    {
        if (quickInventory == null)
        {
            return;
        }

        DeconnectSystem();

        QuickInventoryRef = quickInventory;
        quickInventory.InventoryChanged += RefreshSlot;
        InitSlots(quickInventory.Capacity, GetRT((int)RTs.QuickSlots));
    }

    public void DeconnectSystem()
    {
        if (QuickInventoryRef != null)
        {
            QuickInventoryRef.InventoryChanged -= RefreshSlot;
            QuickInventoryRef = null;
        }
    }

    private void RefreshSlot(IQuickable quickable, int index)
    {
        _slots[index].Refresh(quickable);
    }

    private void InitSlots(int capacity, Transform parent)
    {
        for (int index = 0; index < capacity; index++)
        {
            var quickSlot = Managers.Resource.Instantiate<UI_QuickSlot>("UI_QuickSlot.prefab");
            quickSlot.Index = index;
            quickSlot.transform.SetParent(parent);
        }

        _slots = parent.GetComponentsInChildren<UI_QuickSlot>();
    }
}
