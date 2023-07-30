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

    public GameObject StartingFormationPrefab;
}

[Serializable]
public class WaveData
{
    [FormerlySerializedAs("WaveComponents")]
    public List<Platoon> Platoons;
    public WaveReward WaveReward;
}

[Serializable]
public class WaveReward
{
    public GameObject FormationPrefab;
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