using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MonsterManager : MonoBehaviour {

	public delegate void EnemyActionHandler(Monster m);
	public event EnemyActionHandler EnemySpawned;
	public event EnemyActionHandler EnemyDied;

	public float easyMultiplier = 1, intermediateMultiplier = 2f, expertMultiplier = 3f;

	public float CurrentMultiplier{
		get{ 
			return ValueStore.sharedInstance.level.difficulty == LevelDifficulty.Easy ? 
				easyMultiplier : ValueStore.sharedInstance.level.difficulty == LevelDifficulty.Medium ? intermediateMultiplier : expertMultiplier;
		}
	}
		
	public GameObject deathParticlePrefab;

	public Path[] paths;

	private List<Monster> monstersInScene = new List<Monster>();
	public List<Monster> MonstersInScene {
		get {
			return monstersInScene;
		}
		set {
			monstersInScene = value;
		}
	}

	public List<Monster> enemyPrefabs;

	public TextAsset enemyDetails;

	[HideInInspector] 	public string[] enemyDetailsLines;

	[HideInInspector]	public char enemyDetailsSplit = ';';

	[Header("Star rating Options")]
	public float maximumArmor, maximumHP;

	void Awake(){
		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		enemyDetailsLines = enemyDetails.text.Split (splitFile, StringSplitOptions.None);

		LoadEnemyDetails ();
	}
		
	public void LoadEnemyDetails(){
		if (enemyDetailsLines.Length <= 0)
			return;
		
		foreach (Monster item in enemyPrefabs) {
			foreach (string s in enemyDetailsLines) {
				if (s.Split(enemyDetailsSplit)[0] == string.Concat(item.ID)) {
					string[] details = s.Split (enemyDetailsSplit);
					item.name = details [1];
					item.description = details [2];
					item.MaxHP.BaseValue = float.Parse (details [3]);
					item.Armor.BaseValue = float.Parse (details [4]);
					item.MS.BaseValue = float.Parse (details [5]);
					item.silverValue = float.Parse (details [6]);
					item.livesValue = int.Parse (details [7]);
				}
			}
		}
	}

	public void OnEnemySpawned(Monster m){
		MonstersInScene.Add(m);
		if (EnemySpawned != null) {
			EnemySpawned (m);
		}
	}

	public void OnEnemyDied(Monster m){
		MonstersInScene.Remove(m);
		if (EnemyDied != null) {
			EnemyDied (m);
		}
	}

	public void SpawnEnemy(GameObject prefab, int entrance, int exit){
		// Spawn enemy
		GameObject m = (GameObject)Instantiate (prefab, new Vector3(1000, 1000, 0), Quaternion.identity);
		Monster mon = m.GetComponentInChildren<Monster>();
		// Set Path
		mon.SetPath (entrance, exit);
		// Raise enemyspawned event
		OnEnemySpawned (mon);
	}

	public bool DoesKill(Monster m, float damage, float armorPen){
		float modifiedDamage = damage * m.DamageModifier.Value;
		if (m != null) {
			if (m.currentHealth > 0) {
				if (m.Armor.Value - armorPen > 0) {
					if (m.currentHealth - (float)(modifiedDamage * (50 / (50 + (m.Armor.Value - (armorPen * 2))))) <= 0) {
						return true;
					} else {
						return false;
					}
				} else if (m.currentHealth - modifiedDamage <= 0) {
					return true;
				} else {
					return false;
				}
			} else {
				return true;
			}
		} else {
			return true;
		}
	}
}
