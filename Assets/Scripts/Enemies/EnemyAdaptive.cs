using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AdaptiveAttacker
{
	public System.Type t;
	public int shotCount;
	public float modifier;

	public AdaptiveAttacker(System.Type t, int shotCount){
		this.t = t;
		this.shotCount = shotCount;
	}
}

public class EnemyAdaptive : Monster {

	List<AdaptiveAttacker> attackers = new List<AdaptiveAttacker>();

	public int attacksNeededForReduction;
	public float damageModifierValue;
	public float maxReduction;

	public override void Damage (float damage, float armorpen, DamageSource source, IAttacker killer)
	{
		if (attackers.FirstOrDefault (x => x.t == killer.GetType ()) == null) {
			attackers.Add (new AdaptiveAttacker (killer.GetType (), 1));
			base.Damage (damage, armorpen, source, killer);
		} else {
			AdaptiveAttacker a = attackers.FirstOrDefault (x => x.t == killer.GetType ());
			a.shotCount++;
			if (a.shotCount >= attacksNeededForReduction) {
				a.modifier += damageModifierValue;
				a.shotCount = 0;
				a.modifier = Mathf.Clamp (a.modifier, 0, maxReduction);
			}
			base.Damage (damage * (1 - a.modifier), armorpen, source, killer);
		}
		/*if (killer is ExitPoint) {
			base.Damage (damage * (1 - ), armorpen, source, killer);	
		} else if (killer is RapidArcher) {
			shotsFromRapid += 1;
			if (shotsFromRapid >= attacksNeededForReduction) {
				rapidDamageModifier += damageModifierValue;
				shotsFromRapid = 0;
			}
			rapidDamageModifier = Mathf.Clamp (rapidDamageModifier, 0, 0.7f);
			base.Damage (damage * (1 - rapidDamageModifier), armorpen, source, killer);
		} else if (killer is LongArcher) {
			shotsFromLong += 1;
			if (shotsFromLong >= attacksNeededForReduction) {
				longDamageModifier += damageModifierValue;
				shotsFromLong = 0;
			}
			longDamageModifier = Mathf.Clamp (longDamageModifier, 0, 0.7f);
			base.Damage (damage * (1 - longDamageModifier), armorpen, source, killer);
		} else if (killer is UtilityArcher) {
			shotsFromUtility += 1;
			if (shotsFromUtility >= attacksNeededForReduction) {
				utilityDamageModifier += damageModifierValue;
				shotsFromUtility = 0;
			}
			utilityDamageModifier = Mathf.Clamp (utilityDamageModifier, 0, 0.7f);
			base.Damage (damage * (1 - utilityDamageModifier), armorpen, source, killer);
		} else if (killer is Tower) {
			shotsFromUntrained += 1;
			if (shotsFromUntrained >= attacksNeededForReduction) {
				untrainedDamageModifier += damageModifierValue;
				shotsFromUntrained = 0;
			}
			untrainedDamageModifier = Mathf.Clamp (untrainedDamageModifier, 0, 0.7f);
			base.Damage (damage * (1 - untrainedDamageModifier), armorpen, source, killer);
		} */
	}
}
