using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundEffects
{
    public const string BASE_URL = "SFX/Compositions";

    public static string AMBIENT_1 = $"{BASE_URL}/ambient-1.asset";
    public static string AMBIENT_2 = $"{BASE_URL}/ambient-2.asset";

    public static string LEVEL_START = $"{BASE_URL}/level-start.asset";
    public static string LEVEL_END_VICTORY = $"{BASE_URL}/level-end-victory.asset";
    public static string LEVEL_END_DEFEAT = $"{BASE_URL}/level-end-defeat.asset";

    public static string WAVE_START = $"{BASE_URL}/wave-start.asset";
    public static string WAVE_END = $"{BASE_URL}/wave-end.asset";

    public static string ENEMY_DEATH = $"{BASE_URL}/enemy-death.asset";

    public static string ARCHER_SHOT = $"{BASE_URL}/archer-shot.asset";

    public static string TESTIFICATE = $"{BASE_URL}/testificate.asset";
}
