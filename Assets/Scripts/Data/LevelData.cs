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
public class FormationSpawn
{
    public GameObject FormationPrefab;
    public int FormationSpawnIndex;
}