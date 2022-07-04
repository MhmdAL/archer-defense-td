using System;
using UnityEngine;
using System.Collections;
using System.Timers;

public enum Type {
	ATTACK_SPEED,
	ATTACK_DAMAGE,
	ATTACK_RANGE,
	MOVEMENT_SPEED,
	DAMAGE_TAKEN,
	SLOW_ON_ATTACK,
	ARMOR_PENETRATION,
	Health,
	Armor,
	Status,
	Immunity,
	Other
}

public enum Name {
	AS_BowUpgrade,
	AD_BowUpgrade,
	AR_BowUpgrade,
	Rapid_AtkSpdPerAttack,
	Rapid_Fury,
	Utility_Slow,
	Utility_Vulnerability,
	Utility_Stun,
	Spike_Slow,
	Ability_DamageBoost,
	ArcherAD_Upgrade,
	EnemyBoss1_AtkSlow,
	EnemyRallier_MsIncrease, 
	AtkDmg_IngameUpgrade,
	AtkSpd_IngameUpgrade,
	AtkRng_IngameUpgrade,
	SlowValue_IngameUpgrade,
	EnemyBoss2_ArmorBuff,
	TowerBaseAttackRangeBuff,
	EnemyBoss4_Disablility,
	EnemyBoss5_Immunity,
	EnemyBoss6_Shield
}

public enum StackOperation{
	HighestValue,
	Additive,
	LowestValue
}

public enum ModifierLife{
	Permanent,
	Temporary
}

public class Modifier {

	public Action<IModifiable> ApplyModifier;
	private Action<IModifiable> application;

	public Action<IModifiable> DeApplyModifier;
	private Action<IModifiable> deapplication;

	public ModifierLife lifetime;
	public MyTimer cdTimer;

	public MyTimer CdTimer {
		get {
			return cdTimer;
		}
		set {
			cdTimer = value;
		}
	}

	public bool active = true;
	public float value;
	public Name name;
	public Type type;
	public float currentStack;
	public int intendedLevel;

	public BonusOperation bonusOperation;

	public Modifier(float value, Name name, Type type, BonusOperation bo, int level){
		this.value = value;
		this.name = name;
		this.type = type;
		currentStack = 0;
		this.intendedLevel = level;
		lifetime = ModifierLife.Permanent;
		this.bonusOperation = bo;
	}

	public Modifier(float value, Name name, Type type, BonusOperation bo){
		this.value = value;	
		this.name = name;
		this.type = type;
		currentStack = 0;
		lifetime = ModifierLife.Permanent;
		this.bonusOperation = bo;
	}

	public Modifier(float value, Name name, Type type, BonusOperation bo, MyTimer cdTimer, Action<IModifiable> deapplication = null){
		this.value = value;	
		this.name = name;
		this.type = type;
		currentStack = 0;
		lifetime = ModifierLife.Temporary;
		this.bonusOperation = bo;

		this.cdTimer = cdTimer;
		cdTimer.TimerElapsed += HandleElapsedEventHandler;

		if (deapplication != null) {
			this.DeApplyModifier += deapplication;
		}
	}

	public Modifier(Name name, Type type, float duration, Action<IModifiable> application, Action<IModifiable> deapplication = null){
		this.name = name;
		this.type = type;

		this.cdTimer = ValueStore.sharedInstance.timerManagerInstance.StartTimer(duration);
		cdTimer.TimerElapsed += HandleElapsedEventHandler;

		currentStack = 0;
		lifetime = ModifierLife.Temporary;
		this.application = application;
		this.ApplyModifier += application;
		if (deapplication != null) { 
			this.deapplication = deapplication;
			this.DeApplyModifier += deapplication;
		}
	}

	public void HandleElapsedEventHandler ()
	{		
		active = false;
	} 

	public void Apply(IModifiable m){
		if(ApplyModifier != null){
			ApplyModifier (m);
		}
	}

	public void DeApply(IModifiable m){
		ApplyModifier -= application;
		if (DeApplyModifier != null) {
			DeApplyModifier (m);
			DeApplyModifier -= deapplication;
		}
	}
}

public enum BonusOperation
{
	Flat,
	Percentage,
	OverallMultiplier
}