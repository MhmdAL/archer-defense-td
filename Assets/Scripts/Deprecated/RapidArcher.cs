using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RapidArcher : Tower {

	public float rapidT4Cooldown;

	public float baseAtkSpdPerAtkValue;

	private CooldownTimer rapidT4CooldownTimer;

	private ValueStore vs;

	public override void InitializeValues(){
		base.InitializeValues ();
		rapidT4CooldownTimer = new CooldownTimer (0);

		vs = ValueStore.Instance;
		vs.WaveSpawner.WaveStarted += OnWaveStart;
		vs.WaveSpawner.WaveEnded += OnWaveEnd;
	}

	public void Shoot(Monster target, GameObject projectile, float damage, float armorpen, float radius){
		// if (GetModifier (Name.Rapid_Fury) == null) {
		// 	consecutiveShots += 1;
		// }

		// 	if (GetModifier (Name.Rapid_Fury) == null) {
		// 		AddModifier (new Modifier (baseAtkSpdPerAtkValue + SaveData.GetUpgrade(UpgradeType.Rapid_3).CurrentValue, Name.Rapid_AtkSpdPerAttack, Type.ATTACK_SPEED, BonusType.Percentage,
		// 			ValueStore.Instance.timerManagerInstance.StartTimer(2)), StackOperation.Additive, 5);
		// 	}

		// // Activate Fury if upgraded and off cooldown
		// if (SaveData.GetUpgrade(UpgradeType.Rapid_4).level > 0 && rapidT4CooldownTimer.GetCooldownRemaining() <= 0) {
		// 	if (consecutiveShots >= 8) {
		// 		consecutiveShots = 0;
		// 		if (GetModifier (Name.Rapid_AtkSpdPerAttack) != null) {
		// 			GetModifier (Name.Rapid_AtkSpdPerAttack).active = false;
		// 		}
		// 		//Modifier m = new Modifier (SaveData.GetUpgrade(UpgradeType.Rapid_4).CurrentValue, Name.Rapid_Fury, Type.ATTACK_SPEED, BonusOperation.Percentage,
		// 		//	ValueStore.sharedInstance.timerManagerInstance.StartTimer(5));
		// 		Modifier m = new Modifier (Name.Rapid_Fury, Type.ATTACK_SPEED, 5, ApplyRapidT4Modifier, DeApplyRapidT4Modifier);
		// 		AddModifier (m, StackOperation.HighestValue, 1);
		// 		rapidT4CooldownTimer.ResetTimer (rapidT4Cooldown);	
		// 	}
		// }
	}

	public void OnWaveEnd(int wave){
		rapidT4CooldownTimer.Stop ();
	}

	public void OnWaveStart(int wave){
		rapidT4CooldownTimer.Start ();
	}

	public void ApplyRapidT4Modifier(IModifiable m){
		anim.SetBool ("PlayT4Animation", true);

		ShopUpgrade rT4 = SaveData.GetUpgrade (UpgradeType.Rapid_4);
		ShopUpgrade aT5 = SaveData.GetUpgrade (UpgradeType.All_1);
		AS.multiplier += aT5.level > 0 ? rT4.CurrentValue * (1 + aT5.CurrentValue) : rT4.CurrentValue;
		archerTowerRenderer.color = Color.cyan;
	}

	public void DeApplyRapidT4Modifier(IModifiable m){
		// StartCoroutine (ColorFade());
		anim.SetBool ("PlayT4Animation", false);
	}

	public override void AddStartingModifiers ()
	{
		if (SaveData.GetUpgrade (UpgradeType.Rapid_2).level > 0) {
			// AddModifier (new Modifier (CurrentValue (UpgradeType.Rapid_2), Name.ArcherAD_Upgrade, Type.ATTACK_DAMAGE, BonusType.Percentage), StackOperation.Additive, 1);
		}
	}

	void OnDestroy(){
		vs.WaveSpawner.WaveStarted -= OnWaveStart;
		vs.WaveSpawner.WaveEnded -= OnWaveEnd;
	}
}
