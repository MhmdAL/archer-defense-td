using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public enum LevelDifficulty
{
	Easy = 1,
	Medium = 2,
	Hard = 3
}

[System.Serializable]
public class Level{
	public int levelID;
	public LevelDifficulty difficulty;
	[HideInInspector]	public string levelName;

	[HideInInspector]	public int totalEnemies;
	[HideInInspector]	public int totalEnemiesSlain;

	[HideInInspector]	public bool won;
	[HideInInspector]	public bool maxed;
}

public class LevelButton : MonoBehaviour {
	
	public Level levelToLoad;

	public Level[] requirements;

	[Header("Color.")]
	public Color color;

	[Header("Sprites.")]
	public Sprite defaultSprite;
	public Sprite clickedSprite;
	public Sprite lockedSprite;

	public Image buttonImage;

	public Image progressBarImageBG;
	public Image progressBarImage;

	public GameObject maxedObj;
	public GameObject loadingScreenPrefab;

	public TextMeshProUGUI levelText;

	public Button button;

	private SaveData save;

	void Start(){
		button.onClick.AddListener (OnClick);

		save = DataService.Instance.SaveData;

		SpriteState s = new SpriteState ();
		s.pressedSprite = clickedSprite;
		button.spriteState = s;

		Unlock ();

		progressBarImage.fillAmount = 0;

		Level l = LevelsManager.GetLevel (levelToLoad.levelID, levelToLoad.difficulty);

		if (l == null) {
			Level newLevel = new Level ();
			newLevel.levelID = levelToLoad.levelID;
			newLevel.difficulty = levelToLoad.difficulty;
			newLevel.levelName = levelToLoad.levelName;

			save.levels.Add (newLevel);

			DataService.Instance.WriteSaveData ();
		}

		if (l != null && l.totalEnemies != 0) {
			float fill = (float)l.totalEnemiesSlain / (float)l.totalEnemies;
			progressBarImage.fillAmount = fill;
			if (fill == 1) {
				maxedObj.SetActive (true);
			}
		}
	}

	public void Unlock(){
		bool unlockable = false;
		if (requirements.Length != 0) {
			for (int i = 0; i < requirements.Length; i++) {
				if (DataService.Instance.SaveData.GetLastLevel(requirements [i].difficulty).levelID >= requirements [i].levelID ) {
					unlockable = true;
				} 
			}
		} else {
			if (DataService.Instance.SaveData.GetLastLevel(levelToLoad.difficulty).levelID >= levelToLoad.levelID - 1) {
				unlockable = true;
			}
		}

		if (unlockable) {
			buttonImage.sprite = defaultSprite;
			buttonImage.color = color;
			button.interactable = true;
			levelText.enabled = true;

			progressBarImageBG.gameObject.SetActive (true);
			progressBarImage.gameObject.SetActive (true);
		} else {
			buttonImage.sprite = lockedSprite;
			buttonImage.color = Color.white;
			button.interactable = false;
			levelText.enabled = false;

			progressBarImageBG.gameObject.SetActive (false);
			progressBarImage.gameObject.SetActive (false);
		}
	}

	public void OnClick(){
		SaveData save = DataService.Instance.SaveData;
		Level level = LevelsManager.GetLevel (levelToLoad.levelID, levelToLoad.difficulty);

		LevelsManager.CurrentLevel = level;

		MySceneLoader.instance.LoadScene ("Test");
	}
}
	