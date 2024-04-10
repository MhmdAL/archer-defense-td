using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "audio-comp", menuName = "SFX/AudioComposition")]
public class AudioComposition : ScriptableObject
{
    public List<AudioCompositionPart> Parts;
}

[Serializable]
public class AudioCompositionPart
{
    public AudioCompositionPartType Type;
    public List<ExtendedAudioClip> Clips;
}

[Serializable]
public class ExtendedAudioClip
{
    public AudioClip AudioClip;
    [MinMaxSlider(0, 1, true)]
    public Vector2 VolumeScale = new Vector2(1, 1);
    [MinMaxSlider(-10, 10, true)]
    public Vector2 Pitch  = new Vector2(1, 1);
}

public enum AudioCompositionPartType
{
    PlayAll,
    SelectRandom
}