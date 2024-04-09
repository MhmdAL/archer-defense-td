using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public static class AudioExtensions
{
    public static void PlayOneShot(this AudioSource source, AudioComposition audio)
    {
        var oldPitch = source.pitch;

        var overallPitch = audio.Pitch.RandomConstant();
        var overallVolume = audio.VolumeScale.RandomConstant();

        foreach (var item in audio.ExtendedAudioClips)
        {
            var itemPitch = item.Pitch.RandomConstant();
            var finalPitch = itemPitch * overallPitch;

            var itemVolume = item.VolumeScale.RandomConstant();
            var finalVolume = itemVolume * overallVolume * GlobalManager.GlobalVolumeScale;

            source.pitch = finalPitch;
            source.PlayOneShot(item.AudioClip, finalVolume);
        }

        source.pitch = oldPitch;
    }

    public static float RandomConstant(this Vector2 minMaxCurve)
    {
        return UnityEngine.Random.Range(minMaxCurve.x, minMaxCurve.y);
    }
}

