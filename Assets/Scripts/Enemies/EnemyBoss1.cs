using UnityEngine;
using System.Collections;

public class EnemyBoss1 : Monster {

	public float attackSpeedDebuffValue;
	public float attackSpeedDebuffDuration;

	public override void Damage (float damage, float armorpen, DamageSource source, IAttacker killer)
	{
		base.Damage (damage, armorpen, source, killer);
		Tower t = killer as Tower;
		if (t != null) {
			t.AddModifier (new Modifier (attackSpeedDebuffValue, Name.EnemyBoss1_AtkSlow, Type.ATTACK_SPEED, BonusOperation.Percentage,
				ValueStore.sharedInstance.timerManagerInstance.StartTimer (attackSpeedDebuffDuration)), StackOperation.Additive, 1);
		}
	}
}
