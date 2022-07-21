using UnityEngine;
using UnityTimer;
using System.Linq;

public interface IEnhancement
{
    void Apply(Tower t);
}

public class BerzerkEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var berzerk = t.gameObject.AddComponent<Berzerk>();
        berzerk.BerzerkAtkSpdBuffValue = 3f;
        berzerk.BerzerkDuration = 3f;
        berzerk.Owner = t;
    }
}

public class ExecutionerEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var executeOnHitEffect = ScriptableObject.CreateInstance<ExecuteTargetHitEffect>();

        t.OnHitEffects.Add(executeOnHitEffect);
    }
}

public class HeadshotEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var headshotOnHitEffect = ScriptableObject.CreateInstance<HeadshotTargetHitEffect>();

        t.ExtraData["HeadshotTimer"] = Timer.Register(2f, null, null, false, false, null, false);

        t.OnHitEffects.Add(headshotOnHitEffect);
        t.OnHitEffects.RemoveAll(x => x.GetType() == typeof(NormalDamageTargetHitEffect));
    }
}

public class RampUpEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var rampUpAtkSpd = t.gameObject.AddComponent<RampUpAttackspeed>();
        rampUpAtkSpd.AttackSpeedPerAttack = 0.1f;
        rampUpAtkSpd.RampUpStackLimit = 5;
        rampUpAtkSpd.Owner = t;
    }
}

public class SlowOnAttackEnhancement : IEnhancement
{
    public void Apply(Tower t)
    {
        var slowOnAttackOnHitEffect = ScriptableObject.CreateInstance<SlowTargetHitEffect>();

        t.OnHitEffects.Add(slowOnAttackOnHitEffect);
    }
}

public class MultiShotEnhancement : IEnhancement
{
    private MultiShotEnhancementData _data;

    public MultiShotEnhancement(MultiShotEnhancementData data)
    {
        _data = data;
    }

    public void Apply(Tower t)
    {
        t.SecondaryTargetCount = _data.SecondaryTargetCount;
    }
}

public class MultiShotEnhancementData
{
    public int SecondaryTargetCount { get; set; }
}

public enum EnhancementType
{
    SlowOnAttack,
    RampUp,
    Headshot,
    MultiShot,
    Executioner,
    Berzerk
}
