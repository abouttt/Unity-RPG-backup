using UnityEngine;

public class MiscellaneousItem : StackableItem
{
    public MiscellaneousItemData MiscellaneousData => Data as MiscellaneousItemData;

    public MiscellaneousItem(MiscellaneousItemData data, int count)
        : base(data, count)
    { }
}
