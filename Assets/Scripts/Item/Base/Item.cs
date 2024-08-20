using UnityEngine;

public class Item : IItem
{
    public ItemData Data { get; private set; }

    protected static Inventories s_inventoriesRef;

    public Item(ItemData itemData)
    {
        Data = itemData;
    }

    public static void SetInventoryRef(Inventories inventories)
    {
        s_inventoriesRef = inventories;
    }
}
