using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public List<WaveData> Waves;

    public float DefaultSpawnDelay;
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