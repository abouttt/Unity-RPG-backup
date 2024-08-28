using UnityEngine;

public class HPPotion : ConsumableItem
{
    public HPPotionData HPPotionData => Data as HPPotionData;

    public HPPotion(HPPotionData hpPotionData, int count)
        : base(hpPotionData, count)
    { }

    protected override void OnUse()
    {
        Debug.Log($"Heal {HPPotionData.HealAmount}");
    }
}
