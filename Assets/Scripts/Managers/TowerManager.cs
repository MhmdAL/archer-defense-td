using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class TowerManager : MonoBehaviour {

	public Action TowersInSceneChanged;

	public delegate void ArcherDeployedEventHandler(TowerBase owner);
	public event ArcherDeployedEventHandler ArcherDeployed; 

	public delegate void ArcherSpecialisedEventHandler(ArcherType a);
	public event ArcherSpecialisedEventHandler ArcherSpecialised;

	public List<Tower> TowersInScene{
		get{ 
			return towersInScene;
		}

		set{
			towersInScene = value;
		}
	}

	public List<TowerBase> TowerBasesInScene{
		get{ 
			return towerBasesInScene;
		}

		set{
			towerBasesInScene = value;
		}
	}

	[Header("Prefabs")]
	public Tower untrainedArcherPrefab;
	public Tower rapidArcherPrefab;
	public Tower longArcherPrefab;
	public Tower utilityArcherPrefab;

	public GameObject towerLock;

	public GameObject towerBaseHolder;

	[Header("UI items")]
	public Image archerIcon;
	public Image upgradeCostImage;
	public Image upgradeIcon;

	public Image deployIconImage;
	public Image deployIconArcherImage;

	public Button upgradeButton;
	public Button deployIconButton;

	public TextMeshProUGUI archerTitleText;
	public TextMeshProUGUI archerLevelText;
	public TextMeshProUGUI nextUpgradeText;
	public TextMeshProUGUI nextUpgradeDescText;
	public TextMeshProUGUI upgradeCostText;

	public TextMeshProUGUI deployCostText;

	public GameObject deployIconBG;
	public GameObject deployIconArcher;

	public Color notEnoughSilverButtonColor, defaultButtonColor, defaultTextColor, notEnoughSilverTextColor;

	public Sprite upgradeSprite;
	public Sprite specialityUpgradeSprite;

	[Header("Tower Base Attack Range")]
	public float attackRangeModifier;

	private List<Tower> towersInScene = new List<Tower> ();
	private List<TowerBase> towerBasesInScene = new List<TowerBase> ();

	private ValueStore vs;

	void Awake(){
		foreach (var item in towerBaseHolder.GetComponentsInChildren<TowerBase>()) {
			TowerBasesInScene.Add (item);
		}
	}

    void Start () {
		vs = ValueStore.sharedInstance;
		vs.SilverChanged += OnSilverChanged;
	}
		
	public void CreateTower(TowerBase tb)
	{
		tb.SetState (TowerBaseState.NonClicked);

		if (vs.Silver >= untrainedArcherPrefab.cost)
		{
			Tower t = Instantiate (untrainedArcherPrefab, tb.originalPos, Quaternion.identity) as Tower;
			TowersInScene.Add (t);

			if (ArcherDeployed != null)
				ArcherDeployed (tb);

			if (TowersInSceneChanged != null)
				TowersInSceneChanged ();
			
			t.owner = tb.gameObject;

			if (tb.gameObject.tag == "SuperBase") {
				t.AddModifier (new Modifier (attackRangeModifier, Name.TowerBaseAttackRangeBuff,
					Type.ATTACK_RANGE, BonusOperation.Percentage), StackOperation.Additive, 1);

				t.buffIndicatorPanel.AddIndicator (BuffIndicatorType.ATTACK_RANGE);
			}

			tb.gameObject.SetActive (false);

			vs.Silver -= t.cost;
			vs.buymenu.SetActive(false);

			UpdateTowerDesc (t);
		}
	}

    public void SellTower()
    {
		Tower towerToSell = vs.lastClicked.GetComponent<Tower> ();
		vs.Silver += (int) (towerToSell.silverSpent * 0.6f);

		towerToSell.owner.SetActive (true);

		TowersInScene.Remove (towerToSell);

		if (TowersInSceneChanged != null)
			TowersInSceneChanged ();

		Destroy(vs.lastClicked);

        vs.specialtyMenu.SetActive(false);
		vs.towerDesc.SetActive (false);
    }

	// archerID: 1 = Rapid Archer, 2 = Long Archer, 3 = Utility Archer			
	public void SetSpeciality(int archerID){
		if(ArcherSpecialised != null)
			ArcherSpecialised ((ArcherType) archerID);
		
		Tower t = vs.lastClickedTower;
		if (vs.Silver >= t.UpgradeCost) {
			Tower newTower = null;
			if (archerID == 1) {
				newTower = Instantiate (rapidArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
			} else if (archerID == 2) {
				newTower = Instantiate (longArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
			} else if (archerID == 3) {
				newTower = Instantiate (utilityArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
			}

			TowersInScene.Remove (t);
			TowersInScene.Add (newTower);

			newTower.modifiers.AddRange (t.modifiers);
			newTower.owner = t.owner;

			foreach (var item in t.buffIndicatorPanel.indicators) {
				newTower.buffIndicatorPanel.AddIndicator (item.type, item.cd);
			}

			if (newTower.owner.tag == "SuperBase") {
				newTower.AddModifier (new Modifier (attackRangeModifier, Name.TowerBaseAttackRangeBuff,
					Type.ATTACK_RANGE, BonusOperation.Percentage), StackOperation.Additive, 1);
			}

			newTower.silverSpent = t.cost;
			newTower.Upgrade ();	
			newTower.level += 1;
		
			vs.specialtyMenu.SetActive (false);
			vs.Silver -= t.UpgradeCost;
			vs.lastClicked = newTower.gameObject;
			vs.lastClickedTower = newTower;

			Destroy (t.gameObject);

			UpdateTowerDesc (newTower);
		}
	}

	public void UpdateTowerDesc(Tower t){
		if (t != null) {
			// If next level is unlocked
			if (CanUpgrade (t)) {
				// Remove lock, activate upgradebutton
				towerLock.SetActive (false);
				upgradeButton.interactable = true;
				upgradeCostImage.gameObject.SetActive (true);
			} else { // if next level is locked
				// Activate lock, deactivate upgradebutton
				towerLock.SetActive (true);
				upgradeButton.interactable = false;
				upgradeCostImage.gameObject.SetActive (false);
			}
			// Update archer Icon, Title, Level to match currently clicked archer
			archerIcon.sprite = t.icon;
			archerTitleText.text = t.title;
			archerLevelText.text = "Level  " + string.Concat (t.level);

			// Disable upgrade button and next upgrade details if archer is maxlevel
			if (t.level == t.maxLevel) {
				upgradeIcon.gameObject.SetActive (false);
				nextUpgradeText.text = System.String.Empty;
				nextUpgradeDescText.text = System.String.Empty;
			} else {
				upgradeIcon.gameObject.SetActive (true);
			}

			if (t.level == 0) {
				upgradeIcon.sprite = specialityUpgradeSprite;
				nextUpgradeText.text = System.String.Empty;
				nextUpgradeDescText.text = System.String.Empty;
			} else if (t.level != t.maxLevel){
				upgradeIcon.sprite = upgradeSprite;
				nextUpgradeDescText.text = t.NextUpgradeStats ();
				nextUpgradeText.text = "Next Upgrade";
			}

			upgradeCostText.text = string.Concat(t.UpgradeCost);

			if (vs.Silver >= t.UpgradeCost) {
				upgradeIcon.color = defaultButtonColor;
				upgradeButton.interactable = true;
				upgradeCostText.color = defaultTextColor;
				//if(t.level == 0)
					//upgradeCostText.GetComponent<TextMeshProUGUI> ().color = Color.yellow;
			} else if (vs.Silver < t.UpgradeCost) {
				upgradeIcon.color = notEnoughSilverButtonColor;
				upgradeButton.interactable = false;

				upgradeCostText.color = notEnoughSilverTextColor;
			}
		}
	}

	public void UpdateDeployMenu(){
		if (ValueStore.sharedInstance.Silver >= 50) {
			deployIconImage.color = defaultButtonColor;
			deployIconArcherImage.color = defaultButtonColor;
			deployIconButton.interactable = true;

			deployCostText.color = defaultTextColor;
		} else {
			deployIconImage.color = notEnoughSilverButtonColor;
			deployIconArcherImage.color = notEnoughSilverButtonColor;
			deployIconButton.interactable = false;

			deployCostText.color = notEnoughSilverTextColor;
		}
	}

	public void OnSilverChanged(){
		if(vs.lastClicked != null ){
			if (vs.lastClickType == ClickType.Tower) {
				UpdateTowerDesc (vs.lastClickedTower);
			}else if(vs.lastClickType == ClickType.TowerBase){
				UpdateDeployMenu ();
			}
		}
	}

	public void UpgradeArcher(){
		Tower t = vs.lastClickedTower;
		if (t.archerSpeciality != ArcherType.ClassicArcher) {
			int nextLvl = t.level + 1;

			if (vs.Silver >= t.UpgradeCost && CanUpgrade(t)) {
				t.Upgrade ();
				vs.specialtyMenu.SetActive (false);
				vs.towerDesc.SetActive (false);
				vs.Silver -= t.UpgradeCost;
				t.level += 1;
				UpdateTowerDesc (t);
			}
		} else {
			vs.specialtyMenu.SetActive (true);
		}

	}
		
	public static bool CanUpgrade(Tower t){
		if (t.archerSpeciality == ArcherType.ClassicArcher) 
			return true;
		
		if (t.archerSpeciality == ArcherType.RapidArcher && SaveData.GetUpgrade (UpgradeType.Rapid_1).level >= t.level) {
			return true;
		} else if (t.archerSpeciality == ArcherType.LongArcher && SaveData.GetUpgrade (UpgradeType.Long_1).level >= t.level) {
			return true;
		} else if (t.archerSpeciality == ArcherType.UtilityArcher && SaveData.GetUpgrade (UpgradeType.Utility_1).level >= t.level) {
			return true;
		} else {
			return false;
		}
	}
}

