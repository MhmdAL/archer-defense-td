using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GoldCalculator : MonoBehaviour {

	public TMP_InputField easyLevelField;
	public TMP_InputField mediumLevelField;
	public TMP_InputField hardLevelField;
	public TMP_Text outputField;

	float totalGold;
	int easyLevel;
	int mediumLevel;
	int hardLevel;

	public void ToggleActive(GameObject g){
		g.SetActive (!g.activeSelf);
	}

	public void CalculateGold(){
		SaveData s = DataService.Instance.SaveData;
		totalGold = 0;
		int easyNum = easyLevelField.text != ""? int.Parse (easyLevelField.text) : 0;
		int mediumNum = mediumLevelField.text != ""? int.Parse (mediumLevelField.text) : 0;
		int hardNum = hardLevelField.text != ""? int.Parse (hardLevelField.text) : 0;
		easyLevel = easyNum;
		mediumLevel = mediumNum;
		hardLevel = hardNum;
		for (int i = 0; i < easyNum; i++) {
			totalGold +=  LevelsManager.easyGoldValues[i];
		}

		for (int i = 0; i <  mediumNum; i++) {
			totalGold += LevelsManager.mediumGoldValues [i];
		}

		for (int i = 0; i <  hardNum; i++) {
			totalGold += LevelsManager.hardGoldValues [i];
		}
		outputField.text = "Total Gold:  " + totalGold;
	}

	public void SetAsLastLevel(){
		CalculateGold ();

		DataService.Instance.SaveData.lastLevelEasy.levelID = easyLevel;
		if (easyLevel != 0) {
			DataService.Instance.SaveData.Gold += (int)LevelsManager.easyGoldValues [easyLevel - 1];
		}

		DataService.Instance.SaveData.lastLevelMedium.levelID = mediumLevel;
		if (mediumLevel != 0) {
			DataService.Instance.SaveData.Gold += (int)LevelsManager.mediumGoldValues [mediumLevel - 1];
		}

		DataService.Instance.SaveData.lastLevelHard.levelID = hardLevel;
		if (hardLevel != 0) {
			DataService.Instance.SaveData.Gold += (int)LevelsManager.hardGoldValues [hardLevel - 1];
		}
			
		DataService.Instance.WriteSaveData ();
		LevelsManager.instance.UnlockLevels ();
	}

	public void AddGold(){
		CalculateGold ();
		DataService.Instance.SaveData.Gold = (int)totalGold;

		DataService.Instance.SaveData.lastLevelEasy.levelID = easyLevel;
		DataService.Instance.SaveData.lastLevelMedium.levelID = mediumLevel;
		DataService.Instance.SaveData.lastLevelHard.levelID = hardLevel;
		DataService.Instance.WriteSaveData ();

		LevelsManager.instance.UnlockLevels ();
	}
}
