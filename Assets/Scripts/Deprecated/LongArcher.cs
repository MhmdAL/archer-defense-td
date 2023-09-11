using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongArcher : Tower {

	public Sprite headshotFloatySprite;

	public float baseHeadshotBonusDmg;

	
	public void Shoot(Monster target, GameObject projectile, float damage, float armorpen, float radius){
		float bulletDamage;
		float bulletArmorPen = armorpen;
		if (shotNumber % 3 == 0) {
			ShopUpgrade t5Upgrade = SaveData.GetUpgrade (UpgradeType.All_1);
			ShopUpgrade longT4Upgrade = SaveData.GetUpgrade (UpgradeType.Long_4);
			// set damage to headshot damage
			float t3Damage = damage * (1 + baseHeadshotBonusDmg + SaveData.GetUpgrade (UpgradeType.Long_3).CurrentValue);
			// set damage to headshot damage + percent of enemies health
			float t4Damage = t5Upgrade.level > 0 ? (t3Damage + (longT4Upgrade.CurrentValue
			                 * target.MaxHP.Value)) * (1 + t5Upgrade.CurrentValue) : t3Damage + (longT4Upgrade.CurrentValue * target.MaxHP.Value);
			if (longT4Upgrade.level > 0) {
				bulletDamage = t4Damage;
				//bulletArmorPen += target.Armor.Value / 4;
			} else {
				bulletDamage = t3Damage;
			}

			//audioManager.PlaySound ("Explosion", transform.position);
		} else {
			bulletDamage = damage;
		}

		//audioManager.PlaySound ("Explosion", transform.position);
	}

	public override void OnTargetHit (Monster target, Projectile p, List<Monster> aoeTargets, int shotNumber)
	{
		GameObject g = null;
		if(shotNumber % 3 == 0){
			g = new GameObject ("FloatyHeadshot");
			g.transform.position = target.transform.position;
			g.transform.localScale *= 0.75f;
			FloatyImage f = g.AddComponent<FloatyImage> ();
			SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();
			sr.sortingLayerName = "FloatyImage";
			sr.sprite = headshotFloatySprite;

			f.distance = 8;
			f.speed = 20;
			f.StartFloating ();
		}

		base.OnTargetHit (target, p, aoeTargets, shotNumber);

		if (target != null && g != null) {
			g.transform.SetParent (target.transform);
		}
	}

	public override void AddStartingModifiers ()
	{
		base.AddStartingModifiers ();
		if (SaveData.GetUpgrade (UpgradeType.Long_2).level > 0) {
			// AddModifier (new Modifier (CurrentValue (UpgradeType.Long_2), Name.ArcherAD_Upgrade, Type.ATTACK_DAMAGE, BonusType.Percentage), StackOperation.Additive, 1);			
		}
	}
}
