using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public static class AudioExtensions
{
    public static void PlayOneShot(this AudioSource source, AudioComposition audio)
    {
        foreach (var item in audio.Parts)
        {
            if (item.Type == AudioCompositionPartType.PlayAll)
            {
                foreach (var clip in item.Clips)
                {
                    source.Play(clip);
                }
            }
            else if (item.Type == AudioCompositionPartType.SelectRandom)
            {
                var rand = UnityEngine.Random.Range(0, item.Clips.Count);
                
                source.Play(item.Clips[rand]);
            }
        }
    }

    public static void Play(this AudioSource source, ExtendedAudioClip clip)
    {
        var pitch = clip.Pitch.RandomConstant();

        var volume = clip.VolumeScale.RandomConstant() * GlobalManager.GlobalVolumeScale;

        source.pitch = pitch;
        source.PlayOneShot(clip.AudioClip, volume);
    }

    public static float RandomConstant(this Vector2 minMaxCurve)
    {
        return UnityEngine.Random.Range(minMaxCurve.x, minMaxCurve.y);
    }
}

