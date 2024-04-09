using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class AudioComposition : ScriptableObject
{
    public List<ExtendedAudioClip> ExtendedAudioClips;

    [MinMaxSlider(0, 1, true)]
    public Vector2 VolumeScale;
    [MinMaxSlider(0, 10, true)]
    public Vector2 Pitch;
}

[Serializable]
public class ExtendedAudioClip
{
    public AudioClip AudioClip;
    [MinMaxSlider(0, 1, true)]
    public Vector2 VolumeScale;
    [MinMaxSlider(0, 10, true)]
    public Vector2 Pitch;
}