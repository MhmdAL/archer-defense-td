using UnityEngine;
using System.Collections;

public class AbilityDamageBoost : Ability
{

    public override void InitializeValues()
    {
        vs.towerManagerInstance.TowersInSceneChanged += OnTowersInSceneChange;
        baseCooldown = SaveData.baseUpgradeValues[UpgradeType.DamageBoostCooldown] + SaveData.GetUpgrade(UpgradeType.DamageBoostCooldown).CurrentValue;

        cd = new CooldownTimer(0);
    }

    public override void UpdateReadiness()
    {
        base.UpdateReadiness();
        if (vs.towerManagerInstance.TowersInScene.Count <= 0)
        {
            SetReady(false);
        }
        else if (cd.GetCooldownRemaining() <= 0)
        {
            SetReady(true);
        }
    }

    public void OnTowersInSceneChange()
    {
        UpdateReadiness();
    }

    public override void Activate()
    {
        base.Activate();

        float damageBoostValue = SaveData.baseUpgradeValues[UpgradeType.DamageBoostValue] +
                                 SaveData.GetUpgrade(UpgradeType.DamageBoostValue).CurrentValue;

        float damageBoostDuration = SaveData.baseUpgradeValues[UpgradeType.DamageBoostDuration] +
                                    SaveData.GetUpgrade(UpgradeType.DamageBoostDuration).CurrentValue;

        foreach (Tower item in vs.towerManagerInstance.TowersInScene)
        {
            item.AD.Modify(damageBoostValue, BonusOperation.Percentage, BuffNames.DMG_BOOST_ABILITY, damageBoostDuration);

            item.buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_DAMAGE, new CooldownTimer(damageBoostDuration));
        }
    }
}
