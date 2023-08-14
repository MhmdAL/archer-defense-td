using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public float StartingSilver;
    public int StartingLives;

    public List<WaveData> Waves;

    public float DefaultSpawnDelay;

    public List<FormationSpawn> StartingFormations;
}

[Serializable]
public class WaveData
{
    [FormerlySerializedAs("WaveComponents")]
    public List<Platoon> Platoons;
    public List<FormationSpawn> WaveRewards;
}

[Serializable]
public class FormationSpawn
{
    public GameObject FormationPrefab;
    public int FormationSpawnIndex;
}

[Serializable]
public class Platoon
{
    public float DelayTillNextComponent;

    public List<Squad> Squads;
}

[Serializable]
public class Squad
{
    public GameObject Prefab;
    public int Count;
    public float SpawnDelay;

    public int EntranceId;
    public int ExitId;
}