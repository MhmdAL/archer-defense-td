using UnityEngine;

[CreateAssetMenu(menuName = "Data/AudioProfile")]
public class AudioProfile : ScriptableObject
{
    public AudioClip bow_draw;
    public AudioClip bow_shoot;
    public AudioClip target_hit;
    public AudioClip wave_start;
    public AudioClip wave_end;
    public AudioClip archer_upgrade;
    public AudioClip enemy_walk;
    public AudioClip enemy_killed;
    public AudioClip level_start;
    public AudioClip level_victory;
    public AudioClip level_defeat;
    public AudioClip ambient;
    public AudioClip ambient_wind;
}
