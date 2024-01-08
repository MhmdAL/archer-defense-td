using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioUtils : MonoBehaviour
{
    private static Dictionary<AudioSource, float> volumeDict = new Dictionary<AudioSource, float>();

    public static void FadeOutAllSounds(float duration = 1f)
    {
        var audioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            volumeDict[audioSource] = audioSource.volume;

            audioSource
                .DOFade(0, duration)
                .SetUpdate(true);
        }
    }

    public static void FadeInAllSounds(bool useManagedVolume = true, float duration = 1f)
    {
        var audioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            var targetVolume = audioSource.volume;

            if (useManagedVolume)
                volumeDict.TryGetValue(audioSource, out targetVolume);

            audioSource.volume = 0;

            audioSource
                .DOFade(targetVolume, duration)
                .SetUpdate(true);
        }
    }
}
