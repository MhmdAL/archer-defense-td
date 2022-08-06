using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

[System.Serializable]
public class Wave
{
	public WaveComponent[] wc;
}
[System.Serializable]
public class WaveComponent{
	public GameObject enemyPrefab;
	public int num;
	public float spawnDelay;
	public float delayTillNextComponent;
	public int entrance, exit;
}

public class WaveManager : MonoBehaviour {

    public Wave[] waveInstance;

	public Action<int> WaveStarted;
	public Action<int> WaveEnded;

	[HideInInspector]	public int totalWaves;
	[HideInInspector]	public int curWave;
	[HideInInspector]	public int enemiesRemainingThisWave;
	[HideInInspector]	public int totalEnemies = 0;
	[HideInInspector]	public int totalEnemiesSlain = 0;

	public float defaultSpawnDelay;

	public GameObject spawnButton;

	[HideInInspector]	public bool waveActive = false;

	private float lastWaveSilver;
	private int lastWave;
	private int lastWaveLives;

    void Start()
    {
		ValueStore.Instance.monsterManagerInstance.EnemyDied += OnEnemyDied;

		totalWaves = waveInstance.Length;
		enemiesRemainingThisWave = 0;
        curWave = 0;

		foreach (var item in waveInstance) {
			foreach (var item2 in item.wc) {
				totalEnemies += item2.num;
			}
		}
		ValueStore.Instance.UpdateStats ();
    }

	public void OnEnemyDied(Monster m, DamageSource source){
		if(!m.IsDead)
			enemiesRemainingThisWave -= 1;
		
		if (enemiesRemainingThisWave == 0 && curWave != totalWaves) { // WaveEnded
			waveActive = false;

			if (WaveEnded != null) {
				WaveEnded (curWave);
			}

			spawnButton.SetActive (true);
		} 
	}

    public void SpawnWave()
    {
		// Set wave status to active
		waveActive = true;

		curWave += 1;
		lastWaveSilver = ValueStore.Instance.Silver;
		lastWave = curWave - 1;
		lastWaveLives = ValueStore.Instance.lives;

		// Raise WaveStarted event
		if (WaveStarted != null) {
			WaveStarted (curWave);
		}

		// Find total num of enemies this wave
		foreach (WaveComponent w in waveInstance[curWave - 1].wc) {
			enemiesRemainingThisWave += w.num;
		} 

		// Disable spawn button
        spawnButton.SetActive(false);

		// Start spawning
		StartCoroutine(Spawn());   
    }

    IEnumerator Spawn()
    {
		int cw = curWave - 1;
		for (int i = 0; i < waveInstance[cw].wc.Length; i++) {
			for(int j = 0; j < waveInstance[cw].wc[i].num; j++){
				
				ValueStore.Instance.monsterManagerInstance.SpawnEnemy (waveInstance [curWave - 1].wc [i].enemyPrefab,
					waveInstance [curWave - 1].wc [i].entrance, waveInstance [curWave - 1].wc [i].exit);
				if (waveInstance [cw].wc [i].spawnDelay == 0) {
					yield return new WaitForSeconds (defaultSpawnDelay);
				} else {
					yield return new WaitForSeconds(waveInstance [cw].wc [i].spawnDelay);
				}
			}
			yield return new WaitForSeconds(waveInstance [cw].wc [i].delayTillNextComponent);
		}
    }
}
