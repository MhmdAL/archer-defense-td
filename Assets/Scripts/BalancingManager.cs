using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

public class BalancingManager : MonoBehaviour {

	[Header("DropDowns")]
	public BalancingSpawnDropdown enemyDropdown;
	[Header("Spawning")]
	public TMP_InputField waveField;
	public GameObject spawnButton;

	void Update(){
		spawnButton.SetActive (true);
	}

	public void ToggleActive(GameObject g){
		g.SetActive (!g.activeSelf);
	}

	public void SpawnEnemy(){
		ValueStore.Instance.monsterManagerInstance.SpawnEnemy (enemyDropdown.monsterPrefabs [enemyDropdown.d.value], 0, 0);
	}

	public void ClearEnemies(){
		foreach (var item in FindObjectsOfType<Monster>().ToList()) {
			Destroy (item.transform.root.gameObject);
		}
	}

	public void ClearDpsTexts(){
		foreach (var item in FindObjectsOfType<BalancingDpsText>().ToList()) {
			item.damageValue = 0;
			item.textObject.text = item.damageValue + "";
		}
	}

	public void SetWave(){
		ValueStore.Instance.WaveSpawner.CurrentWave = int.Parse (waveField.text);
	}
}
