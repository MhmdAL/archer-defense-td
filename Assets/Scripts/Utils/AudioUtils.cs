using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioUtils : MonoBehaviour
{
    private static Dictionary<AudioSource, float> volumeDict = new Dictionary<AudioSource, float>();

    public static void FadeOutAllSounds()
    {
        var audioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            volumeDict[audioSource] = audioSource.volume;

            audioSource
                .DOFade(0, 1f)
                .SetUpdate(true);
        }
    }

    public static void FadeInAllSounds()
    {
        var audioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            volumeDict.TryGetValue(audioSource, out var targetVolume);

            audioSource
                .DOFade(targetVolume, 1f)
                .SetUpdate(true);
        }
    }
}
