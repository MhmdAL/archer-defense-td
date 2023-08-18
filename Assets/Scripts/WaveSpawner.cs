using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public event Action<int> WaveStarted;
    public event Action<int> WaveEnded;

    public LevelData LevelData { get; set; }

    public int TotalWaves => LevelData.Waves.Count;
    public int TotalEnemies => LevelData.Waves.Sum(x => x.Platoons.Sum(y => y.Squads.Sum(z => z.Count)));

    public int CurrentWave { get; set; }
    public int EnemiesRemainingInCurrentWave { get; private set; }

    public bool IsFinished => EnemiesRemainingInCurrentWave == 0 && CurrentWave == TotalWaves;

    private List<IEnumerator> _activeRoutines = new List<IEnumerator>();

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

        Debug.Log("WaveSpawner: EnemyDied");

        if (EnemiesRemainingInCurrentWave == 0)
        {
            WaveEnded?.Invoke(CurrentWave);
        }
    }

    public void SpawnNextWave()
    {
        CurrentWave++;

        EnemiesRemainingInCurrentWave = LevelData.Waves[CurrentWave - 1].Platoons.Sum(x => x.Squads.Sum(y => y.Count));

        WaveStarted?.Invoke(CurrentWave);

        var routine = SpawnWave(LevelData.Waves[CurrentWave - 1]);

        _activeRoutines.Add(routine);

        StartCoroutine(routine);
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var platoon in wave.Platoons)
        {
            for (int i = 0; i < platoon.Squads.Count; i++)
            {
                StartCoroutine(SpawnSquad(platoon.Squads[i]));
            }

            yield return new WaitForSeconds(platoon.DelayTillNextComponent);
        }
    }

    private IEnumerator SpawnSquad(Squad squad)
    {
        for (int i = 0; i < squad.Count; i++)
        {
            SpawnEnemy(squad.Prefab, squad.EntranceId, squad.ExitId);

            if (squad.SpawnDelay == 0)
            {
                yield return new WaitForSeconds(LevelData.DefaultSpawnDelay);
            }
            else
            {
                yield return new WaitForSeconds(squad.SpawnDelay);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab, int entranceId, int exitId)
    {
        ValueStore.Instance.monsterManagerInstance.SpawnEnemy(prefab, entranceId, exitId);
    }

    public void Reset(LevelData levelData)
    {
        if (_activeRoutines.Any())
        {
            _activeRoutines.ForEach(x => StopCoroutine(x));
            _activeRoutines.Clear();
        }

        this.LevelData = levelData;

        CurrentWave = 0;
        EnemiesRemainingInCurrentWave = 0;
    }
}
