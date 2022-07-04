using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour {

	public static LevelsManager instance;

	public GameObject easyPanel;
	public GameObject mediumPanel;
	public GameObject hardPanel;

	public GameObject easySelector;
	public GameObject mediumSelector;
	public GameObject hardSelector;

	public GameObject archerInfoButton;

	public GameObject endGameMenu;

	public TextAsset levelsText;

	public Sprite lockIcon;
	public Sprite levelIcon;

	public RectTransform scrollRectContentTransform;

	public int archerInfoButtonUnlockLevel;

	[NonSerialized]	public static List<float> easyGoldValues = new List<float>();
	[NonSerialized] public static List<float> mediumGoldValues = new List<float>();
	[NonSerialized] public static List<float> hardGoldValues = new List<float>();

	[NonSerialized]	public static Level CurrentLevel;

	private Vector3 difficultySelectionOriginalPosition;

	private LevelButton[] levelButtons;


	void Awake(){
		Time.timeScale = 1;

		instance = this;

		AssignLevels ();
	}

	void Start(){
		difficultySelectionOriginalPosition = scrollRectContentTransform.anchoredPosition;

		SaveData sd = DataService.Instance.SaveData;
		Level x = GetLevel (archerInfoButtonUnlockLevel);
		if(x.won){
			archerInfoButton.SetActive(true);
		}else{
			archerInfoButton.SetActive(false);
		}
		archerInfoButton.transform.SetAsLastSibling ();

		if (GetLevel (30, LevelDifficulty.Hard).won && !sd.endGameShown) {
			print ("x");
			endGameMenu.SetActive (true);
			sd.endGameShown = true;

			DataService.Instance.WriteSaveData ();
		}
	}
		
	public void ActivatePanel(int panel){
		if (panel == 1) {
			easyPanel.SetActive (true);
			mediumPanel.SetActive (false);
			hardPanel.SetActive (false);

			easySelector.SetActive (true);
			mediumSelector.SetActive (false);
			hardSelector.SetActive (false);
		} else if (panel == 2) {
			easyPanel.SetActive (false);
			mediumPanel.SetActive (true);
			hardPanel.SetActive (false);

			easySelector.SetActive (false);
			mediumSelector.SetActive (true);
			hardSelector.SetActive (false);
		} else if (panel == 3) {
			easyPanel.SetActive (false);
			mediumPanel.SetActive (false);
			hardPanel.SetActive (true);

			easySelector.SetActive (false);
			mediumSelector.SetActive (false);
			hardSelector.SetActive (true);
		}
		AssignLevels ();
		ResetScrollRectPosition ();
	}

	public void ResetScrollRectPosition(){
		scrollRectContentTransform.anchoredPosition = difficultySelectionOriginalPosition;
	}

	public static void LoadGoldValues(TextAsset goldValues){
		// Clear lists that hold gold values for each level
		easyGoldValues.Clear ();
		mediumGoldValues.Clear ();
		hardGoldValues.Clear ();

		// re-add gold values to the lists
		string[] goldsPerDifficulty = goldValues.text.Split ('/');
		for (int i = 0; i < goldsPerDifficulty.Length; i++) {
			string[] difficultyLevelsSplit = goldsPerDifficulty [i].Split (':');
			string[] goldsPerLevel = difficultyLevelsSplit [1].Split ('+');
			foreach (var item in goldsPerLevel) {
				if (difficultyLevelsSplit [0].Contains("Easy")) {
					easyGoldValues.Add (int.Parse (item));
				} else if (difficultyLevelsSplit [0].Contains("Medium")) {
					mediumGoldValues.Add (int.Parse (item));
				} else if (difficultyLevelsSplit [0].Contains("Hard")) {
					hardGoldValues.Add (int.Parse (item));
				}
			}
		}
	}

	public static Level GetLevel(int id, LevelDifficulty diff){
		Level l = DataService.Instance.SaveData.levels.FirstOrDefault(x => x.levelID == id && x.difficulty == diff);
		return l;
	}

	public static Level GetLevel(int id){
		Level l = DataService.Instance.SaveData.levels.FirstOrDefault(x => x.levelID == id);
		return l;
	}

	public static void LevelWon(ref Level l){
		SaveData sd = DataService.Instance.SaveData;
		if (l.levelID >= sd.GetLastLevel(l.difficulty).levelID) {
			sd.SetLastLevel (l);
		}

		DataService.Instance.WriteSaveData ();
	}

	public void UnlockLevels(){
		foreach (LevelButton item in levelButtons) {
			item.Unlock ();
		}
	}

	public void AssignLevels(){
		levelButtons = FindObjectsOfType<LevelButton> ();

		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		string[] lines = levelsText.text.Split (splitFile, StringSplitOptions.None);
		for (int i = 0; i < lines.Length; i++) {
			string[] levelDescAndReq = lines [i].Split ('/');
			string[] levelDesc = levelDescAndReq [0].Split(':');
			LevelButton lb = levelButtons.FirstOrDefault (x => x.levelToLoad.levelID == int.Parse (levelDesc [0]));
			if (lb != null) {
				lb.levelToLoad.levelName = levelDesc [1];
				lb.levelText.text = levelDesc [1];
			}
		}
	}
}
