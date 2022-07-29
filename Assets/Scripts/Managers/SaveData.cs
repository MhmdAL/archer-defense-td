using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class SaveData {

	public Action GoldChanged;

	public const Level DEFAULT_LEVEL = null;
	public const int DEFAULT_GOLD = 0;
	public const float DEFAULT_AD = 10;
	public const float DEFAULT_AS = .5f;
	public const float DEFAULT_AR = 16f;
	public const float DEFAULT_LIVES = 10;

	public const float DEFAULT_ARTILLERY_COOLDOWN = 40;
	public const int DEFAULT_ARTILLERY_ARROWCOUNT = 3;

	public const float DEFAULT_DAMAGEBOOST_VALUE = 2.5f;
	public const float DEFAULT_DAMAGEBOOST_COOLDOWN = 40;
	public const float DEFAULT_DAMAGEBOOST_DURATION = 8;

	public static Dictionary<UpgradeType, float> baseUpgradeValues = new Dictionary<UpgradeType, float> (){
		{ UpgradeType.Lives, DEFAULT_LIVES }, { UpgradeType.ArtilleryCooldown, DEFAULT_ARTILLERY_COOLDOWN },
		{ UpgradeType.ArtilleryArrowCount, (float)DEFAULT_ARTILLERY_ARROWCOUNT }, { UpgradeType.DamageBoostValue, DEFAULT_DAMAGEBOOST_VALUE }, 
		{ UpgradeType.DamageBoostCooldown, DEFAULT_DAMAGEBOOST_COOLDOWN }, { UpgradeType.DamageBoostDuration, DEFAULT_DAMAGEBOOST_DURATION }
	};

	public List<ShopUpgrade> upgradeList;

	public List<Level> levels = new List<Level>();

	public Level lastLevelEasy = DEFAULT_LEVEL;
	public Level lastLevelMedium = DEFAULT_LEVEL;
	public Level lastLevelHard = DEFAULT_LEVEL;

	[SerializeField]
	private int gold = DEFAULT_GOLD;

	public int Gold {
		get {
			return gold;
		}
		set {
			gold = value;
			OnGoldChanged ();
		}
	}

	public bool endGameShown;

	public SaveData(){
		TextAsset levelsText = (TextAsset)Resources.Load ("Levels.txt");

		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		string[] lines = levelsText.text.Split (splitFile, StringSplitOptions.None);
		for (int i = 0; i < lines.Length; i++) {
			string[] levelDescAndReq = lines [i].Split ('/');
			string[] levelDesc = levelDescAndReq [0].Split(':');

			for (int j = 1; j < 4; j++) {
				Level x = new Level ();
				x.levelID = int.Parse(levelDesc[0]);
				x.levelName = levelDesc[1];
				x.difficulty = (LevelDifficulty)j;

				levels.Add (x);
			}
		}


	}
		
	public void WriteToFile(string path){
		string json = JsonUtility.ToJson (this, true);

		File.WriteAllText (path, json);
	}

	private void OnGoldChanged(){
		if (GoldChanged != null)
			GoldChanged ();
	}

	public Level GetLastLevel(LevelDifficulty l){
		return l == LevelDifficulty.Easy ? lastLevelEasy : l == LevelDifficulty.Medium ? lastLevelMedium : lastLevelHard;
	}

	public void SetLastLevel(Level l){
		if (l.difficulty == LevelDifficulty.Easy) {
			lastLevelEasy = l;
		} else if (l.difficulty == LevelDifficulty.Medium) {
			lastLevelMedium = l;
		} else if (l.difficulty == LevelDifficulty.Hard) {
			lastLevelHard = l;
		}
	}

	public static SaveData ReadFromFile(string path){

		if (!File.Exists (path)) {
			return new SaveData ();
		} else {
			string contents = File.ReadAllText (path);

			if (string.IsNullOrEmpty (contents)) {
				return new SaveData ();
			}
			return JsonUtility.FromJson<SaveData> (contents);
		}
	}

	public bool isDefault(){
		return(
		    lastLevelEasy == DEFAULT_LEVEL &&
			Gold == DEFAULT_GOLD);
	}

	public void Reset(){
		DataService.Instance.SaveData.Gold = DEFAULT_GOLD;
		DataService.Instance.WriteSaveData ();
	}

	public static ShopUpgrade GetUpgrade(UpgradeType x){
		ShopUpgrade ss = null;
		foreach (ShopUpgrade s in DataService.Instance.SaveData.upgradeList) {
			if (s.ID.Equals(x)) {
				ss = s;
				return ss;
			}
		}
		return ss;
	}
}
