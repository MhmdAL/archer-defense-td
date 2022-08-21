using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public float StartingSilver;
    public int StartingLives;

    public List<WaveData> Waves;

    public SerTest2 XD;

    public float DefaultSpawnDelay;
}

[Serializable]
public class SerTest2
{
    public int xD;
}

[Serializable]
public class WaveData
{
    public List<WaveComponentData> WaveComponents;
}

[Serializable]
public class WaveComponentData
{
    public GameObject Prefab;
    public int Count;
    public float SpawnDelay;
    public float DelayTillNextComponent;

    public int EntranceId;
    public int ExitId;
}

[Serializable]
public class Squad
{
    public GameObject Prefab;
    public int Count;
    public float SpawnDelay;
    public float DelayTillNextComponent;

    public int EntranceId;
    public int ExitId;
}