using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilityData/HorseRaid")]
public class HorseRaidAbilityData : AbilityData
{
    public int HorseRaiderCount = 3;
    public float HorseRaiderSpawnDelay = 1;

    public GameObject HorseRaiderPrefab;

    [Header("SFX")]
    public AudioComposition raidStartSFX;
}
