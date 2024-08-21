using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment", fileName = "Equipment_")]
public class EquipmentItemData : ItemData, ILevelLimitableItem
{
    [field: Header("Equipment Data")]
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; } = 1;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    public EquipmentItemData()
        : base(ItemType.Equipment)
    { }

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
