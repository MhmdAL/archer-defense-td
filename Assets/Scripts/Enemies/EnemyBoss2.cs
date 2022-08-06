using UnityEngine;
using System.Collections;

public class EnemyBoss2 : Monster {

	float shotNumber;
	public float armorBuffCooldown;
	public float armorBuffDuration;
	public float armorBuffValue;
	public int armorBuffShotsNeeded;

	CooldownTimer armorBuffCooldownTimer;

	public override void InitializeValues ()
	{
		base.InitializeValues ();
		armorBuffCooldownTimer = new CooldownTimer (0);
	}

	public override void Damage (float damage, float armorpen, DamageSource source, IAttacker killer)
	{
		base.Damage (damage, armorpen, source, killer);
		if (killer is Tower) {
			shotNumber += 1;
		}
		if (armorBuffCooldownTimer.GetCooldownRemaining () <= 0) {
			if (shotNumber >= armorBuffShotsNeeded) {
				anim.SetBool ("ArmorGlow", true);
				AddModifier (new Modifier (armorBuffValue, Name.EnemyBoss2_ArmorBuff, Type.Armor,
					BonusOperation.Percentage, ValueStore.Instance.timerManagerInstance.StartTimer (armorBuffDuration), DeApplyArmorBuff), StackOperation.Additive, 1);
				shotNumber = 0;
				armorBuffCooldownTimer.ResetTimer (armorBuffCooldown);
			}
		}
	}

	public void DeApplyArmorBuff(IModifiable m){
		anim.SetBool ("ArmorGlow", false);
	}
}
