using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

public enum UpgradeType{
	AD = 0,
	AS = 1,
	AR = 2,
	AP = 3,
	Poison_Arrows = 4,
	Multiiple_Arrows = 5,
	Rapid_1 = 6,
	Rapid_2 = 7,
	Rapid_3 = 8,
	Rapid_4 = 9,
	Long_1 = 10,
	Long_2 = 11,
	Long_3 = 12,
	Long_4 = 13,
	Utility_1 = 14,
	Utility_2 = 15,
	Utility_3 = 16,
	Utility_4 = 17, 
	All_1 = 18,
	Lives = 19,
	ArtilleryDamage = 20,
	ArtilleryCooldown = 21,
	ArtilleryArrowCount = 22,
	DamageBoostValue = 23,
	DamageBoostCooldown = 24,
	DamageBoostDuration = 25,
	SilverIncrease = 26,
}
[Serializable]
public class ShopUpgrade  {
	
	public UpgradeType ID;

	[HideInInspector]	public int nID;
	[HideInInspector]	public float startingUpgradeValue;
	[HideInInspector]	public int level;
	[HideInInspector]	public int maxLevel;
	[HideInInspector]	public CostOperation costOperation;
	[HideInInspector]	public int baseCost;
	[HideInInspector] 	public int costPerLevel;
	[HideInInspector]	public int CurrentCost{
		get { 
			if (costOperation == CostOperation.NormalStack) {
				return (int) (baseCost + (costPerLevel * (level) ));
			} else if (costOperation == CostOperation.x2) {
				return (int) ((baseCost) + (costPerLevel * Mathf.Pow (2, level)));
			} else {
				return 0;
			}
		}
	} 
	[HideInInspector]	public float valuePerLevel;
	[HideInInspector]	public string requirements;
	[HideInInspector]	public bool isPercentage;
	[NonSerialized]
	[HideInInspector]	public string upgradeTitle, upgradeDesc;

	private float currentValue;
	public float CurrentValue {
		get {
			if (level > 0)
				return startingUpgradeValue + level * valuePerLevel;
			else
				return startingUpgradeValue;
		}
	}

	private TextAsset text;

	[HideInInspector]	public bool showStats;

	public ShopUpgrade (int nID, float baseValue, int maxLevel, int baseCost, int costPerLevel, float valuePerLevel, string requirements, bool isPercentage, int co, bool showStats){
		this.nID = nID;
		ID = (UpgradeType)nID;
		this.level = 0;
		this.startingUpgradeValue = baseValue;
		this.maxLevel = maxLevel;
		costOperation = (CostOperation)co;
		this.baseCost = baseCost;
		this.costPerLevel = costPerLevel;
		this.valuePerLevel = valuePerLevel;
		this.requirements = requirements;
		this.isPercentage = isPercentage;
		this.showStats = showStats;

		text = (TextAsset)Resources.Load ("1.txt");
		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		string[] lines = text.text.Split (splitFile, StringSplitOptions.None);
		string[] realtext = lines [nID].Split (';'); 
		upgradeTitle = realtext [0];
		upgradeDesc = realtext [1];
	}

	public float ValueAtLevel(int lvl){
		if (lvl > 0)
			return startingUpgradeValue + lvl * valuePerLevel;
		else
			return startingUpgradeValue;
	}
}

public enum CostOperation
{
	NormalStack,
	x2
}
