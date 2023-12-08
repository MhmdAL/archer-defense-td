using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    /// <summary>
    /// Called when a platoon is commencing spawn now
    /// </summary>
    public event Action<Platoon> PlatoonSpawned;
    /// <summary>
    /// Called when wave starts. 1-based wave number provided
    /// </summary>
    public event Action<int> WaveStarted;
    /// <summary>
    /// Called when wave ends. 1-based wave number provided
    /// </summary>
    public event Action<int> WaveEnded;

    public LevelData LevelData { get; set; }

    public int TotalWaves => LevelData.Waves.Count;
    public int TotalEnemies => LevelData.Waves.Sum(x => x.Platoons.Sum(y => y.Squads.Sum(z => z.Count)));

    /// <summary>
    /// 1-based wave index
    /// </summary>
    public int CurrentWave { get; set; }
    public int EnemiesRemainingInCurrentWave { get; private set; }

    public bool IsFinished => EnemiesRemainingInCurrentWave == 0 && CurrentWave == TotalWaves;

    public float WaveTime { get; private set; }

    public bool IsSpawning { get; private set; }

    public Platoon CurrentPlatoon { get; set; }
    public Platoon NextPlatoon { get; set; }

    public float TimeTillNextPlatoon { get; set; }

    private List<IEnumerator> _activeRoutines = new List<IEnumerator>();

    [SerializeField]
    private AudioSource upcomingWaveAudioSource;

    [SerializeField]
    private AudioClip upcomingWaveSFX;

    private List<Monster> _spawnedMonsters = new List<Monster>();


    private void Start()
    {
        CurrentWave = 0;
    }

    private void Update()
    {
        WaveTime += Time.deltaTime;

        // print(TimeTillNextPlatoon);
    }

    public void OnEnemyDied(Monster m, DamageSource source)
    {
        EnemiesRemainingInCurrentWave--;

        Debug.Log("WaveSpawner: EnemyDied");

        if (EnemiesRemainingInCurrentWave == 0)
        {
            WaveEnded?.Invoke(CurrentWave);
        }
    }


    public void SpawnNextWave()
    {
        if (CurrentWave >= LevelData.Waves.Count)
        {
            Debug.LogWarning("Max wave reached, can't start next wave");
            return;
        }

        WaveTime = 0;

        CurrentWave++;

        EnemiesRemainingInCurrentWave += LevelData.Waves[CurrentWave - 1].Platoons.Sum(x => x.Squads.Sum(y => y.Count));

        WaveStarted?.Invoke(CurrentWave);

        var routine = SpawnWave(LevelData.Waves[CurrentWave - 1]);

        _activeRoutines.Add(routine);

        StartCoroutine(routine);
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        IsSpawning = true;

        for (int i = 0; i < wave.Platoons.Count; i++)
        {
            var platoon = wave.Platoons[i];

            CurrentPlatoon = platoon;
            NextPlatoon = wave.Platoons.Count > i + 1 ? wave.Platoons[i + 1] : null;

            for (int j = 0; j < platoon.Squads.Count; j++)
            {
                StartCoroutine(SpawnSquad(platoon.Squads[j]));
            }

            this.TimeTillNextPlatoon = platoon.DelayTillNextPlatoon;

            PlatoonSpawned?.Invoke(platoon);

            while (TimeTillNextPlatoon > 0)
            {
                TimeTillNextPlatoon -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        IsSpawning = false;
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
        var monster = ValueStore.Instance.monsterManagerInstance.SpawnEnemy(prefab, entranceId, exitId);

        _spawnedMonsters.Add(monster);

        monster.OnDeath += (unit, ds) => OnEnemyDied(unit as Monster, ds);
    }

    // For testing
    public void SetWave(int wave)
    {
        if (wave >= LevelData.Waves.Count || wave < 0)
        {
            Debug.LogWarning("Invalid wave number");
            return;
        }

        if (_activeRoutines.Any())
        {
            _activeRoutines.ForEach(x => StopCoroutine(x));
            _activeRoutines.Clear();
        }

        if (_spawnedMonsters.Any())
        {
            foreach (var mon in _spawnedMonsters.ToList())
            {
                if (mon == null) continue;

                Destroy(mon.transform.parent.gameObject);
            }

            _spawnedMonsters.Clear();
        }

        WaveTime = 0;

        CurrentWave = wave;

        EnemiesRemainingInCurrentWave = 0;
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
