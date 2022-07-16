using UnityEngine;

public interface IEnhancement
{
    void Apply(Tower t);
}

public class RampUpEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var rampUpAtkSpd = t.gameObject.AddComponent<RampUpAttackspeed>();
        rampUpAtkSpd.AttackSpeedPerAttack = 0.75f;
        rampUpAtkSpd.RampUpStackLimit = 5;
        rampUpAtkSpd.Owner = t;
    }
}

public class SlowOnAttackEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var slowOnAttackOnHitEffect = ScriptableObject.CreateInstance<ApplyPoisonTargetHitEffect>();

        t.OnHitEffects.Add(slowOnAttackOnHitEffect);
    }
}

public class MultiShotEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        t.SecondaryTargetCount = 5;
    }
}

public enum EnhancementType
{
    SlowOnAttack,
    RampUp,
    MultiShot
}
