using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat {

	public Stat(Type t){
		type = t;
	}

	[System.NonSerialized]	public Type type;

	[System.NonSerialized]	public bool locked = false;

	public float baseValue = 0;
	[System.NonSerialized]	public float bonusValue = 0;

	public float BonusValue {
		get {
			bonusValue = baseValue * multiplier + flatBonus;
			return bonusValue;
		}
		set{ 
			bonusValue = value;
			
		}
	}
	[System.NonSerialized]	public float multiplier = 0;
	[System.NonSerialized]	public float overallMultiplier = 1;
	[System.NonSerialized]	public float flatBonus = 0;
	float totalValue;

	public float Value {
		get {
			if (!locked) {
				totalValue = baseValue + BonusValue;
				return totalValue * overallMultiplier;
			} else {
				return totalValue * overallMultiplier;
			}
		}
		set {
			if (!locked) {
				totalValue = value;
			}
		}
	}
}
