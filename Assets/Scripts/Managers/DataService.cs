using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class DataService : MonoBehaviour {

	private static DataService _instance = null;
	public static DataService Instance{
		get{ 
			if (_instance == null) {
				_instance = FindObjectOfType<DataService> ();

				if (_instance == null) {
					GameObject go = new GameObject (typeof(DataService).ToString ());
					_instance = go.AddComponent<DataService> ();

					_instance.LoadSaveData(1);
				}
			}

			return _instance;
		}
	}

	public TextAsset goldValues;
	public TextAsset shopUpgrades;

	public SaveData SaveData{ get; private set;}

	bool isDataLoaded = false;

	public int currentlyLoadedProfileNumber{ get; private set;}

	public const int MAX_NUMBER_OF_PROFILES = 1;

	private const string SAVE_DATA_FILE_NAME_BASE = "ad_save";

	private const string  SAVE_DATA_FILE_EXTENSION = ".MhmdAL";

	private string SAVE_DATA_DIRECTORY{ get { return Application.persistentDataPath + "/saves/"; } }

	void Awake() {
		if (Instance != this) {
			Destroy (this);
		} else {
			DontDestroyOnLoad (gameObject);
		}
	}
		
	void Update(){
		if (Input.GetKeyDown (KeyCode.Q)) {
			Debug.Log ("dasd");
			SaveData.Reset ();
		}
	}

	void OnLevelWasLoaded(){
		if (SaveData == null)
			LoadSaveData (1);
		
		Scene activeScene = SceneManager.GetActiveScene ();
		if (activeScene.buildIndex > 1) {
		//	SaveData.lastLevel = activeScene.path.Replace ("Assets/", "").Replace (".unity/", "");
		}

		LoadGoldValues();

		LoadShopUpgrades();

		WriteSaveData ();
	}

	public void LoadGoldValues(){
		LevelsManager.LoadGoldValues (goldValues);
	}

	public void LoadShopUpgrades(){
		// Create temporary list
		List<ShopUpgrade> upgradeList = new List<ShopUpgrade> ();

		// If first time playing with this SaveData, create new list of upgrades
		if (SaveData.upgradeList == null) {
			SaveData.upgradeList = new List<ShopUpgrade> ();
		}

		// Load text containing details for each upgrade
		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		string[] lines = shopUpgrades.text.Split (splitFile, StringSplitOptions.None);
		for (int i = 0; i < lines.Length; i++) {
			string[] realtext = lines [i].Split (';');

			ShopUpgrade s = new ShopUpgrade (int.Parse (realtext [0]), float.Parse (realtext [1]), int.Parse (realtext [2]), int.Parse (realtext [3]),
				int.Parse (realtext [4]), float.Parse (realtext [5]), realtext [6], bool.Parse (realtext [7]), int.Parse (realtext [8]), bool.Parse(realtext[9]));

			// If upgrade exists in list then copy the current level 
			if (SaveData.GetUpgrade (s.ID) != null) {
				s.level = SaveData.GetUpgrade (s.ID).level;
			} else { // If not then add a new one
				SaveData.upgradeList.Add (s);
			}

			upgradeList.Add (s);
		}

		// Set upgradelist to the new temporary list
		SaveData.upgradeList = upgradeList;
	}
		
	public void LoadSaveData(int profileNumber = 0){
		if (isDataLoaded && profileNumber == currentlyLoadedProfileNumber)
			return;

		if (profileNumber <= 0) {
			UnityEngine.Debug.Log ("profile > 0");
			for (int i = 1; i <= MAX_NUMBER_OF_PROFILES; i++) {
				UnityEngine.Debug.Log ("file exists");

				if (File.Exists (GetSaveDataFilePath (i))) {

					SaveData = SaveData.ReadFromFile (GetSaveDataFilePath (i));
					currentlyLoadedProfileNumber = i;
					break;
				}
			}
		} else {
			if (File.Exists (GetSaveDataFilePath (profileNumber))) {
				SaveData = SaveData.ReadFromFile (GetSaveDataFilePath (profileNumber));
			} else {
				SaveData = new SaveData ();
			}

			currentlyLoadedProfileNumber = profileNumber;
		}
	}

	public void WriteSaveData(){
		if (currentlyLoadedProfileNumber <= 0) {
			for (int i = 1; i <= MAX_NUMBER_OF_PROFILES; i++) {
				if (!File.Exists (GetSaveDataFilePath (i))) {
					currentlyLoadedProfileNumber = i;
					break;
				}
			}
		}

		if (currentlyLoadedProfileNumber <= 0) {
			throw new System.Exception ("Cannot WriteSaveData. No available profiles and currentlyLoadedProfile = 0");
		} else {
			if (SaveData == null)
				SaveData = new SaveData ();

			SaveData.WriteToFile (GetSaveDataFilePath (currentlyLoadedProfileNumber));
		}
	}

	public string GetSaveDataFilePath(int profileNumber){
		if (profileNumber < 1) {
			throw new System.ArgumentException ("profile number must be greater than 1" + profileNumber);
		}

		if (!Directory.Exists (SAVE_DATA_DIRECTORY)) {
			Directory.CreateDirectory (SAVE_DATA_DIRECTORY);
		}

		return SAVE_DATA_DIRECTORY + SAVE_DATA_FILE_NAME_BASE + profileNumber.ToString () + SAVE_DATA_FILE_EXTENSION;
	}
}
