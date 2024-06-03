using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class WaveData : ScriptableObject
{
    public List<Platoon> Platoons;
    public List<FormationSpawn> WaveRewards;
}


[Serializable]
public class Platoon
{
    [FormerlySerializedAs("DelayTillNextComponent")]
    public float DelayTillNextPlatoon;

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