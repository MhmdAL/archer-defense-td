using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyBoss5 : EnemyAdaptive {

	public float immunityCooldown;
	public float immunityDuration;
	public float damagePercentTillImmunity;

	CooldownTimer immunityTimer;

	bool immune = false;

	public override void InitializeValues ()
	{
		base.InitializeValues ();
		immunityTimer = new CooldownTimer (0);
	}
	
	public override void FixedUpdate ()
	{
		base.FixedUpdate ();
		if (immunityTimer.GetCooldownRemaining () <= 0) {
			if (CurrentHP <= damagePercentTillImmunity * MaxHP.Value) {
				//AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
				//	immunityDuration * 2, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
				anim.SetBool("PlayInvulnerability", true);
				immunityTimer.ResetTimer (immunityCooldown);
			}else if (CurrentHP <= damagePercentTillImmunity * 2 * MaxHP.Value) {
				//AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
				//	immunityDuration * 1.5f, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
				anim.SetBool("PlayInvulnerability", true);
				immunityTimer.ResetTimer (immunityCooldown);
			}else if (CurrentHP <= damagePercentTillImmunity * 3 * MaxHP.Value) {
			//	AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
			//		immunityDuration, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
				anim.SetBool("PlayInvulnerability", true);
				immunityTimer.ResetTimer (immunityCooldown);
			}
		}
	}

	public void ActivateImmunity(){
		anim.SetBool("PlayInvulnerability", false);

		anim.SetFloat("InvulnerabilitySpeed", 0);
		

		if (CurrentHP <= damagePercentTillImmunity * MaxHP.Value) {
			// AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
			// 	immunityDuration * 2, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
		}else if (CurrentHP <= damagePercentTillImmunity * 2 * MaxHP.Value) {
			// AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
			// 	immunityDuration * 1.5f, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
		}else if (CurrentHP <= damagePercentTillImmunity * 3 * MaxHP.Value) {
			// AddModifier (new Modifier (Name.EnemyBoss5_Immunity, Type.Immunity,
			// 	immunityDuration, ApplyImmunityModifier, DeApplyImmunityModifier), StackOperation.Additive, 1);
		}
	}

	public override void Damage (float damage, float armorpen, DamageSource source, IAttacking killer, DamageMetaData damageMeta)
	{
		if (!immune || killer is ExitPoint) {
			base.Damage (damage, armorpen, source, killer, damageMeta);
		} 
	}

	public void ApplyImmunityModifier(IModifiable m){
		immune = true;
	}

	public void DeApplyImmunityModifier(IModifiable m){
		anim.SetFloat("InvulnerabilitySpeed", 1);
		immune = false;
	}
}
