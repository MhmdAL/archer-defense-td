using UnityEngine;

[CreateAssetMenu(fileName = "StatModifyPotionEffect", menuName = "Data/StatModifyPotionEffect")]
public class StatModifyPotionEffect : PotionEffect
{
    public StatEffect StatToAffect;
    public float PercentValue;

    public override void Apply(Tower target)
    {
        if (StatToAffect == StatEffect.AD)
        {
            target.AD.Modify(PercentValue, BonusType.Percentage, this.GetInstanceID().ToString());
        }
        else if (StatToAffect == StatEffect.AR)
        {
            target.AR.Modify(PercentValue, BonusType.Percentage, this.GetInstanceID().ToString());
        }
        else if (StatToAffect == StatEffect.AS)
        {
            target.AS.Modify(PercentValue, BonusType.Percentage, this.GetInstanceID().ToString());
        }
    }
}

public enum StatEffect
{
    AD,
    AR,
    AS
}