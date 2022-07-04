using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

public class ShopHandler : MonoBehaviour {

	public static ShopHandler sharedInstance;

	public delegate void UpgradedHandler(ShopUpgrade s);
	public event UpgradedHandler Upgraded;

	public GameObject upgradeMenu, upgradeButton, requirementsMenu;

	public TextMeshProUGUI goldText;

	public TextMeshProUGUI upgradeLevelText;
	public TextMeshProUGUI upgradeTitleText;
	public TextMeshProUGUI upgradeDescText;
	public TextMeshProUGUI upgradeCostText;
	public TextMeshProUGUI upgradeValueText;

	public TextMeshProUGUI requirementsTitleText;
	public TextMeshProUGUI requirementsText;

	public GameObject bowButton, archersButton, baseButton;

	[HideInInspector]	public UpgradeType lcType;
	[HideInInspector]	public ShopButton lastClickedShopButton;


	void Awake(){
		sharedInstance = this;	

		DataService.Instance.SaveData.GoldChanged += OnGoldChanged;

		UpdateGold ();
	}

	void Update(){
		if (Input.GetKey (KeyCode.Space)) {
			int totalcost = 0;

			foreach (var item in DataService.Instance.SaveData.upgradeList) {
				for (int i = 0; i < item.maxLevel; i++) {
					totalcost += item.baseCost + (item.costPerLevel * i);
				}
			}

			float totalGold = 0;
			foreach (var item in LevelsManager.easyGoldValues) {
				totalGold += item;
			}
			foreach (var item in LevelsManager.mediumGoldValues) {
				totalGold += item;
			}
			foreach (var item in LevelsManager.hardGoldValues) {
				totalGold += item;
			}

			print ("Total Gold Gained: " + totalGold + ".. Total Cost: " + totalcost);

			DataService.Instance.WriteSaveData ();
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			Level l = new Level ();
			l.levelID = 1;
			LevelsManager.CurrentLevel = l;
			SceneManager.LoadScene ("Test Scene 2");

			DataService.Instance.WriteSaveData ();
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			SaveData.GetUpgrade (UpgradeType.AD).level = 11;

			DataService.Instance.WriteSaveData ();
		} else if (Input.GetKey (KeyCode.UpArrow)) {
			DataService.Instance.SaveData.Gold += 100;

			DataService.Instance.WriteSaveData ();
		}else if (Input.GetKey (KeyCode.DownArrow)) {
			DataService.Instance.SaveData.Gold -= 100;

			DataService.Instance.WriteSaveData ();
		}else if (Input.GetKey (KeyCode.M)) {
			foreach (var item in DataService.Instance.SaveData.upgradeList) {
				item.level = item.maxLevel;
			}

			DataService.Instance.WriteSaveData ();
		}
	}

	public void OnGoldChanged(){
		UpdateGold ();
	}

	public void UpdateGold(){
		goldText.text = string.Concat(DataService.Instance.SaveData.Gold);
	}

	public void OnUpgraded(ShopUpgrade s){
		if(Upgraded != null)
			Upgraded (s);
	}

	public void Load(string levelToLoad){
		SceneManager.LoadScene (levelToLoad);
	}

	public void OpenUpgradeMenu(ShopButton x){
		upgradeMenu.SetActive (true);
		upgradeButton.SetActive (true);
		lcType = x.type;
		UpdateMenuText ();
	}
		
	public void UpgradeBow(){
		SaveData save = DataService.Instance.SaveData;
		ShopUpgrade upgrade = SaveData.GetUpgrade (lcType);

		int cost = upgrade.CurrentCost;

		if (save.Gold >= cost && upgrade.level < upgrade.maxLevel) {
			upgrade.level += 1;
			print (upgrade.CurrentValue);
			OnUpgraded (upgrade);
			save.Gold -= cost;
		}
			
		UpdateMenuText ();

		DataService.Instance.WriteSaveData ();
	}

	public void CloseMenu(GameObject x){
		x.SetActive (false);
	}

	public void UpdateMenuText(){
		ShopUpgrade lastUpgrade = SaveData.GetUpgrade (lcType);

		upgradeTitleText.text = lastUpgrade.upgradeTitle;

		upgradeDescText.text = lastUpgrade.upgradeDesc;

		// Set menu level indicator to currently selected upgrade and match color
		upgradeLevelText.text = lastUpgrade.level + "/" + lastUpgrade.maxLevel;
		upgradeLevelText.enableVertexGradient = lastClickedShopButton.levelIndicatorText.enableVertexGradient;
		upgradeLevelText.color = lastClickedShopButton.levelIndicatorText.color;
		upgradeLevelText.colorGradient = lastClickedShopButton.levelIndicatorText.colorGradient;
		upgradeLevelText.outlineColor = lastClickedShopButton.levelIndicatorText.outlineColor;

		// Show price and set upgrade value accordingly if upgrade isn't maxed out
		if (lastUpgrade.level < lastUpgrade.maxLevel) {
			upgradeCostText.text = string.Concat(lastUpgrade.CurrentCost);

			if (lastUpgrade.showStats) {
				float curValue = SaveData.baseUpgradeValues.ContainsKey (lastUpgrade.ID) ? SaveData.baseUpgradeValues [lastUpgrade.ID] +
					lastUpgrade.CurrentValue : lastUpgrade.level == 0 ? 0 : lastUpgrade.CurrentValue; 
				float nextValue = SaveData.baseUpgradeValues.ContainsKey (lastUpgrade.ID) ? SaveData.baseUpgradeValues [lastUpgrade.ID] +
					lastUpgrade.ValueAtLevel(lastUpgrade.level + 1) : lastUpgrade.ValueAtLevel(lastUpgrade.level + 1); 
				
				if (lastUpgrade.isPercentage) {
					upgradeValueText.text = "Current: <color=#00ffff>" + Math.Round (curValue * 100, 1)
						+ "%</color>" + "\nNext: <color=#00ffff>" + Math.Round (nextValue * 100, 1) + "%</color>";
				} else {
					upgradeValueText.text = "Current: <color=#00ffff>" + Math.Round (curValue, 1)
						+ "</color>\nNext: <color=#00ffff>" + Math.Round (nextValue, 1) + "</color>";
				}
			} else {
				upgradeValueText.text = "";
			}
		}else{ // Hide price and upgrade button and set upgrade value accordingly if upgrade is maxed out
			upgradeButton.SetActive (false);

			upgradeCostText.text = System.String.Empty;

			if (lastUpgrade.showStats) {
				float value = SaveData.baseUpgradeValues.ContainsKey (lastUpgrade.ID) ? SaveData.baseUpgradeValues [lastUpgrade.ID] +
				              lastUpgrade.CurrentValue : lastUpgrade.CurrentValue; 
				if (lastUpgrade.isPercentage) {
					upgradeValueText.text = "Current: <color=#00ffff>" +
					Math.Round (value * 100, 1) + "%    </color><color=green>MAX!</color>";
				} else {
					upgradeValueText.text = "Current: <color=#00ffff>"
						+ Math.Round (value, 1) + "    </color><color=green>MAX!</color>";
				}
			}else {
				upgradeValueText.text = "";
			}
		}
	}
		
	public void OpenRequirementsMenu(ShopButton x, string requirements, string title){
		requirementsMenu.SetActive (true);
		requirementsText.text = requirements;
		requirementsTitleText.text = title;
	}
}
