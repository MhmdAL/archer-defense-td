using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public enum ClickType{
	Background,
	TowerBase,
	Tower
}

public enum GameStatus{
	Win,
	Loss
}
[ExecuteInEditMode]
public class ValueStore : MonoBehaviour {

	public static ValueStore sharedInstance;

	public static float CurrentTime;

	public delegate void SilverChangedEventHandler ();
	public event SilverChangedEventHandler SilverChanged;

	public delegate void GameEndedHandler(GameStatus s);
	public event GameEndedHandler GameEnded;

	[HideInInspector]	public GameObject lastClicked;
	[HideInInspector]	public Tower lastClickedTower;
	[HideInInspector]	public ClickType lastClickType;

	public GameObject entranceNodePrefab, exitNodePrefab;

    public GameObject buymenu, specialtyMenu, towerDesc, endGameMenu, pauseMenu;
	public GameObject allEnemiesSlainEndMenuGoldDesc;

	public TowerManager towerManagerInstance;
	public MonsterManager monsterManagerInstance;
	public WaveManager waveManagerInstance;
	public AudioManager audioManagerInstance;
	public TimerManager timerManagerInstance;
	public AbilityManager abilityManagerInstance;
	public InfoPanelManager infoPanelManagerInstance;
	public InfoBoxManager infoBoxManagerInstance;
	public GameUIController uiControllerInstance;

	public TextMeshProUGUI levelPopUpText;

	public TextMeshProUGUI endGameMenuGoldText;
	public TextMeshProUGUI endGameMenuSlainGoldText;
	public TextMeshProUGUI endGameMenuTotalGoldText;
	public TextMeshProUGUI endGameMenuWonText;
	public TextMeshProUGUI endGameMenuTitleText;

	public TextMeshProUGUI livesText;
	public TextMeshProUGUI waveText;
	public TextMeshProUGUI silverText;

	public float startingSilver;

	private float silver;

	public float Silver {
		get {
			return silver;
		}
		set {
			silver = value;
			OnSilverChange ();
		}
	}

	private float easyGoldValue, intermediateGoldValue, expertGoldValue;
	public float CurrentGoldValue{
		get{ 
			return level.difficulty == LevelDifficulty.Easy ? easyGoldValue : level.difficulty == LevelDifficulty.Medium ? intermediateGoldValue : expertGoldValue;
		}
	}

	[HideInInspector] 	public float silverEarned;

	[HideInInspector]	public int lives;

	[HideInInspector]	public Level level;

	[HideInInspector] 	public bool active;

	[HideInInspector]	public Camera mainCamera;

	private ArcherDeployMenu adm;

    void Awake () {
		active = true;
		sharedInstance = this;
		if (Application.isPlaying) {
			waveManagerInstance.WaveStarted += OnWaveStart;
			monsterManagerInstance.EnemyDied += OnEnemyDeath;

			adm = buymenu.GetComponent<ArcherDeployMenu> ();

			SaveData s = DataService.Instance.SaveData;

			level = LevelsManager.CurrentLevel;
			easyGoldValue = LevelsManager.easyGoldValues [level.levelID - 1];
			intermediateGoldValue = LevelsManager.mediumGoldValues [level.levelID - 1];
			expertGoldValue = LevelsManager.hardGoldValues [level.levelID - 1];
			Silver = startingSilver;
			lives = (int)(SaveData.DEFAULT_LIVES + SaveData.GetUpgrade (UpgradeType.Lives).CurrentValue);

			levelPopUpText.text = level.levelName;

			mainCamera = Camera.main;

			UpdateStats ();
		}
	}

	void Update(){
		CurrentTime = Time.time;
	}

	public void OnClick(ClickType c, GameObject clicker){
		lastClicked = clicker;
		if (c == ClickType.Background) {
			lastClickedTower = null;
			lastClickType = ClickType.Background;

			buymenu.SetActive(false);
			specialtyMenu.SetActive(false);
			towerDesc.SetActive (false);

		} else if (c == ClickType.TowerBase) {
			lastClickedTower = null;
			lastClickType = ClickType.TowerBase;

			adm.transform.position = new Vector3(10000, 10000, 0);
			adm.objToFollow = lastClicked;
			adm.tb = lastClicked.GetComponent<TowerBase> ();

			buymenu.SetActive (true);
			buymenu.transform.SetAsLastSibling ();
			pauseMenu.transform.SetAsLastSibling ();

			specialtyMenu.SetActive (false);
			towerDesc.SetActive (false);

			uiControllerInstance.UpdateDeployMenu ();
		} else if (c == ClickType.Tower) {
			lastClickType = ClickType.Tower;
			buymenu.SetActive(false);

			uiControllerInstance.UpdateTowerDesc (lastClickedTower);
			specialtyMenu.SetActive (false);
			towerDesc.SetActive (true);

		}
	}

	public void OnSilverChange(){
		if (SilverChanged != null)
			SilverChanged ();
		UpdateStats ();
	}

	public void OnWaveStart(int wave){
		UpdateStats ();
	}

	public void OnEnemyDeath(Monster enemy){
		UpdateStats ();
	}

	public void AnchorsToCorners(RectTransform t){
		RectTransform pt = t.parent as RectTransform;

		if(t == null || pt == null) return;

		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}

	public void UpdateStats(){
		livesText.text = string.Concat (lives);

		waveText.text = waveManagerInstance.curWave + "/" + waveManagerInstance.totalWaves;

		silverText.text = string.Concat (Silver);
	}

	public void LoadLevel(string levelToLoad)
    {
		SceneManager.LoadScene (levelToLoad);
    }

	public void RestartLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

    public void addGold()
    {
        //Silver += 1000;
    }

	public void GameOver(GameStatus gs){
		active = false;
		if (GameEnded != null)
			GameEnded (gs);
		Time.timeScale = 0f;

		SaveData save = DataService.Instance.SaveData;

		float winGoldGained = 0f;
		float allEnemiesSlainGoldGained = 0f;
		float totalGoldGained = 0f;

		if (gs == GameStatus.Win) {
			endGameMenuTitleText.text = "Victory!";
			endGameMenuWonText.text = "Victory:-";
			// If level is won for the first time, set enemy slain count and gold gain accordingly
			if (!level.won) {
				level.totalEnemies = waveManagerInstance.totalEnemies;
				if (level.totalEnemiesSlain < waveManagerInstance.totalEnemiesSlain) {
					level.totalEnemiesSlain = waveManagerInstance.totalEnemiesSlain;
				}

				winGoldGained = (float)(CurrentGoldValue * 0.8f) + ((CurrentGoldValue * 0.2f) * ((float)lives / (10 + SaveData.GetUpgrade (UpgradeType.Lives).level)));
				level.won = true;
			} 
			// If level is already won, set enemy slain count and gold gain accordingly
			else {
				if (level.totalEnemiesSlain < waveManagerInstance.totalEnemiesSlain) {
					level.totalEnemiesSlain = waveManagerInstance.totalEnemiesSlain;
				}

				winGoldGained = (CurrentGoldValue * 0.2f) * ((float)lives / (10 + SaveData.GetUpgrade (UpgradeType.Lives).level));
			}

			// If level is maxed for the first time, set gold gain accordingly
			if (!level.maxed && level.totalEnemiesSlain == level.totalEnemies) {
				allEnemiesSlainGoldGained = 0.4f * CurrentGoldValue;
				allEnemiesSlainEndMenuGoldDesc.SetActive (true);
				level.maxed = true;
			} 
				
			LevelsManager.LevelWon (ref level);
		} else if (gs == GameStatus.Loss) {
			endGameMenuTitleText.text = "Defeat";
			endGameMenuWonText.text = "Defeat:-";

			endGameMenuTitleText.color = Color.red;
			winGoldGained = (0.15f * CurrentGoldValue * ((float)waveManagerInstance.curWave / (float)waveManagerInstance.totalWaves));
		}

		totalGoldGained = winGoldGained + allEnemiesSlainGoldGained;

		endGameMenuGoldText.text = "+ " + Mathf.CeilToInt(winGoldGained);
		endGameMenuSlainGoldText.text = "+ " + Mathf.CeilToInt(allEnemiesSlainGoldGained);
		endGameMenuTotalGoldText.text = "+ " + Mathf.CeilToInt(totalGoldGained);

		save.Gold += Mathf.CeilToInt (totalGoldGained);

		DataService.Instance.WriteSaveData ();

		endGameMenu.SetActive (true);
	}

    public void ExitGame()
    {
		Application.Quit ();
    }
}
