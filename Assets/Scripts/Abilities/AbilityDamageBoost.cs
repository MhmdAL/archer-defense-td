﻿using UnityEngine;
using System.Collections;
using UnityTimer;

public class AbilityDamageBoost : Ability
{

    public override void Initialize()
    {
        vs.towerManagerInstance.TowersInSceneChanged += OnTowersInSceneChange;
        // baseCooldown = SaveData.baseUpgradeValues[UpgradeType.DamageBoostCooldown] + SaveData.GetUpgrade(UpgradeType.DamageBoostCooldown).CurrentValue;
        baseCooldown = 10;

        CooldownTimer = this.AttachTimer(0, null, isDoneWhenElapsed: false);
    }

    public override void UpdateReadiness()
    {
        if (vs.towerManagerInstance.TowersInScene.Count <= 0)
        {
            SetReady(false);
        }
        else if (CooldownTimer.GetTimeRemaining() <= 0)
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
        float damageBoostValue = SaveData.baseUpgradeValues[UpgradeType.DamageBoostValue] +
                                 SaveData.GetUpgrade(UpgradeType.DamageBoostValue)?.CurrentValue ?? 0;

        float damageBoostDuration = SaveData.baseUpgradeValues[UpgradeType.DamageBoostDuration] +
                                    SaveData.GetUpgrade(UpgradeType.DamageBoostDuration)?.CurrentValue ?? 0;

        foreach (Tower item in vs.towerManagerInstance.TowersInScene)
        {
            item.AD.Modify(damageBoostValue, BonusOperation.Percentage, BuffNames.DMG_BOOST_ABILITY, damageBoostDuration);

            item.buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_DAMAGE, new CooldownTimer(damageBoostDuration));
        }
    }
}
