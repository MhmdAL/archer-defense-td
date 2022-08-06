using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public event Action<int> WaveStarted;
    public event Action<int> WaveEnded;

    public LevelData LevelData;

    public int TotalWaves => LevelData.Waves.Count;
    public int TotalEnemies => LevelData.Waves.Sum(x => x.WaveComponents.Sum(y => y.Count));

    public int CurrentWave { get; set; }
    public int EnemiesRemainingInCurrentWave { get; private set; }

    public bool IsFinished => EnemiesRemainingInCurrentWave == 0 && CurrentWave == TotalWaves;

    private void Start()
    {
        CurrentWave = 0;
    }

    public void OnEnemyDied(Monster m, DamageSource source)
    {
        if (!m.IsDead)
        {
            EnemiesRemainingInCurrentWave--;
        }

        if (EnemiesRemainingInCurrentWave == 0)
        {
            WaveEnded?.Invoke(CurrentWave);
        }
    }

    public void SpawnNextWave()
    {
        CurrentWave++;

        EnemiesRemainingInCurrentWave = LevelData.Waves[CurrentWave - 1].WaveComponents.Sum(x => x.Count);

        WaveStarted?.Invoke(CurrentWave);

        StartCoroutine(SpawnWave(LevelData.Waves[CurrentWave - 1]));
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var component in wave.WaveComponents)
        {
            for (int i = 0; i < component.Count; i++)
            {
                SpawnEnemy(component.Prefab, component.EntranceId, component.ExitId);

                if (component.SpawnDelay == 0)
                {
                    yield return new WaitForSeconds(LevelData.DefaultSpawnDelay);
                }
                else
                {
                    yield return new WaitForSeconds(component.SpawnDelay);
                }
            }

            yield return new WaitForSeconds(component.DelayTillNextComponent);
        }
    }

    private void SpawnEnemy(GameObject prefab, int entranceId, int exitId)
    {
        ValueStore.Instance.monsterManagerInstance.SpawnEnemy(prefab, entranceId, exitId);
    }
}
