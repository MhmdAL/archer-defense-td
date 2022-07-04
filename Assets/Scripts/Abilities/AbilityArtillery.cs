using UnityEngine;
using System.Collections;

public class AbilityArtillery : Ability {

	public GameObject arrowPrefab;
	public Transform startPos;

	public float arrowSpeed;
	public float cooldownPerArrow;

	private const float ArtilleryBaseDamage = 15;

	public override void InitializeValues ()
	{
		baseCooldown = SaveData.baseUpgradeValues[UpgradeType.ArtilleryCooldown] + SaveData.GetUpgrade (UpgradeType.ArtilleryCooldown).CurrentValue;

		cd = new CooldownTimer (0);
	}

	public override void UpdateReadiness ()
	{
		base.UpdateReadiness ();

		if (vs.monsterManagerInstance.MonstersInScene.Count <= 0) {
			SetReady (false);
		} else if(cd.GetCooldownRemaining() <= 0){
			SetReady (true);
		}
	}

	public override void Activate ()
	{
		base.Activate ();

		StartCoroutine (StartArtillery ());
	}

	IEnumerator StartArtillery(){
		// Loop over current amount of arrows shot per target
		int arrowCount = (int)(SaveData.baseUpgradeValues[UpgradeType.ArtilleryArrowCount] + 
			SaveData.GetUpgrade(UpgradeType.ArtilleryArrowCount).CurrentValue);

		for (int i = 0; i < arrowCount; i++) {
			// Loop over current monsters in scene
			foreach (Monster m in vs.monsterManagerInstance.MonstersInScene) {
				// Instantiate Arrow and set values
				GameObject arrow = (GameObject)Instantiate (arrowPrefab, startPos.position, Quaternion.identity);
				ArtilleryArrow p = arrow.AddComponent<ArtilleryArrow> ();
				p.owner = this;
				p.damage = (1 + SaveData.GetUpgrade(UpgradeType.ArtilleryDamage).CurrentValue * Mathf.Clamp(vs.monsterManagerInstance.CurrentMultiplier * 0.75f, 1, 5)) * 
					(1 + SaveData.GetUpgrade(UpgradeType.AD).CurrentValue) * ArtilleryBaseDamage;
				p.armorPen = SaveData.GetUpgrade(UpgradeType.AP).CurrentValue;
				p.speed = arrowSpeed;
				p.target = m;
			}
			yield return new WaitForSeconds (cooldownPerArrow);
		}
	}
		
	/*IEnumerator LaunchArtillery(Monster m){
		if (m) {
			for (int i = 0; i < SaveData.GetUpgrade(UpgradeType.ArtilleryArrowCount).CurrentValue; i++) {
				Vector3 pos = startPos.position;
				GameObject arrow = (GameObject)Instantiate (arrowPrefab, pos, Quaternion.identity);
				ArtilleryArrow p = arrow.GetComponentInChildren<ArtilleryArrow> ();
				p.owner = this;
				p.damage = SaveData.GetUpgrade(UpgradeType.ArtilleryDamage).CurrentValue;
				p.speed = arrowSpeed;
				p.target = m;

			}
		}
	}*/
}
