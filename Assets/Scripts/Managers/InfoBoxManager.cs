using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class InfoBoxManager : MonoBehaviour {

	public List<InfoBox> infoBoxes;

	public GameObject blackBg;

	[HideInInspector]	public static bool firstArcherDeployed = false;
	[HideInInspector]	public static bool firstSuperArcherDeployed = false;
	[HideInInspector]	public static bool firstArcherSpecialised = false;
	[HideInInspector]	public static bool firstWaveStarted = false;
	[HideInInspector]	public static bool abilityBoxesShown = false;
	[HideInInspector]	public static bool artilleryUsed = false;
	[HideInInspector]	public static bool damageBoostUsed = false;

	private ValueStore vs;

	void Awake(){
		firstArcherDeployed = false;
		firstSuperArcherDeployed = false;
		firstArcherSpecialised = false;
		firstWaveStarted = false;
		abilityBoxesShown = false;
		artilleryUsed = false;
		damageBoostUsed = false;
	}

	void Start(){
		vs = ValueStore.Instance;

		foreach (var item in infoBoxes) {
			item.ibm = this;
			item.SetDelegates ();
		}

		vs.WaveSpawner.WaveStarted += OnWaveStart;
		vs.towerManagerInstance.TowerDeployed += OnTowerDeployed;
		vs.towerManagerInstance.TowerSpecialised += OnTowerSpecialised;
		Ability.AbilityActivated += OnAbilityActivated;

		UpdateState ();

		UpdateBoxes ();

		foreach (var item in infoBoxes) {
			item.ibm = this;
			item.OnEnable ();
		}

		UpdateBackground ();
	}

	public void UpdateState(){
		foreach (var item in infoBoxes) {
			item.UpdateState ();
		}

		UpdateBackground();
	}

	public void OnTowerInfoDeactivated(){
		foreach (var item in vs.towerManagerInstance.TowerBasesInScene) {
			item.SetLayer ("Default");
		}
	}

	public void OnTowerInfoActivated(){
		foreach (var item in vs.towerManagerInstance.TowerBasesInScene) {
			item.SetLayer ("Highest");
		}
	}

	public bool FirstWaveStarted(){
		if (firstWaveStarted)
			return true;
		return false;
	}

	public bool FirstArcherDeployed(){
		if (firstArcherDeployed)
			return true;
		return false;
	}

	public bool FirstSuperArcherDeployed(){
		if (firstSuperArcherDeployed)
			return true;
		return false;
	}

	public bool FirstArcherSpecialised(){
		if (firstArcherSpecialised)
			return true;
		return false;
	}

	public bool ArtilleryUsed(){
		if (artilleryUsed)
			return true;
		return false;
	}

	public bool DamageBoostUsed(){
		if (damageBoostUsed)
			return true;
		return false;
	}

	public void UpdateBoxes(){
		foreach (var item in infoBoxes) {
			if(item != null)
				item.UpdateVisibility ();
		}
	}
		
	public void UpdateBackground(){
		if (infoBoxes.Count == 0 || infoBoxes.FirstOrDefault(x => x.gameObject.activeInHierarchy) == null){
			blackBg.SetActive (false);
			return;
		}

		InfoBox somebox = infoBoxes.FirstOrDefault (x => x != null && x.gameObject.activeInHierarchy);
		if (somebox == null && blackBg.activeSelf) {
			blackBg.SetActive (false);
		} else if(blackBg != null && !blackBg.activeSelf){
			blackBg.SetActive (true);
		}
	}

	public void OnWaveStart(int waveCount){
		if (!firstWaveStarted) {
			firstWaveStarted = true;
			UpdateBoxes ();
		}
	}

	public void OnTowerDeployed(Tower tower){
		if (tower.TowerBase.tag != "SuperBase")
			OnNormalArcherDeployed ();
		else
			OnSuperArcherDeployed ();
		
		UpdateBoxes ();
	}

	public void OnNormalArcherDeployed(){
		if (!firstArcherDeployed) {
			firstArcherDeployed = true;
		}
	}

	public void OnSuperArcherDeployed(){
		if (!firstSuperArcherDeployed) {
			firstSuperArcherDeployed = true;
		}
	}

	public void OnTowerSpecialised(Tower tower){
		if (!firstArcherSpecialised){
			firstArcherSpecialised = true;
			UpdateBoxes ();
		}
	}

	public void OnAbilityActivated(AbilityType a){
		if (a == AbilityType.Arrow_Artillery) {
			artilleryUsed = true;
		}else if(a == AbilityType.Damage_boost){
			damageBoostUsed = true;
		}
		UpdateBoxes ();
	}

	void OnDestroy(){
		vs.WaveSpawner.WaveStarted -= OnWaveStart;
		vs.towerManagerInstance.TowerDeployed -= OnTowerDeployed;
		vs.towerManagerInstance.TowerSpecialised -= OnTowerSpecialised;
		Ability.AbilityActivated -= OnAbilityActivated;
	}
}
