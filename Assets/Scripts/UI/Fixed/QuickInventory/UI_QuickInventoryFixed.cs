using System.Collections.Generic;
using UnityEngine;

public class UI_QuickInventoryFixed : UI_Base, ISystemConnectable<QuickInventory>
{
    enum RTs
    {
        QuickSlots,
    }

    public QuickInventory QuickInventoryRef { get; private set; }

    private readonly List<UI_QuickSlot> _quickSlots = new();

    protected override void Init()
    {
        BindRT(typeof(RTs));
        Managers.UI.Register(this);
    }

    public void ConnectSystem(QuickInventory quickInventory)
    {
        QuickInventoryRef = quickInventory;
        quickInventory.InventoryChanged += RefreshSlot;
        InitSlots(quickInventory.Quickables.Count, GetRT((int)RTs.QuickSlots));
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
        _quickSlots[index].Refresh(quickable);
    }

    private void InitSlots(int capacity, Transform parent)
    {
        for (int i = 0; i < capacity; i++)
        {
            var quickSlot = Managers.Resource.Instantiate<UI_QuickSlot>("UI_QuickSlot.prefab");
            quickSlot.Setup(i);
            quickSlot.transform.SetParent(parent);
            _quickSlots.Add(quickSlot);
        }
    }
}
