using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ArcherSpecialtyData : ScriptableObject
{
    [field: SerializeField]
    public List<ArcherSpecialtyValues> SpecialtyValues { get; set; }
}

[Serializable]
public struct ArcherSpecialtyValues
{
    [field: SerializeField]
    public int ExpRequired { get; set; }
    [field: SerializeField]
    public float ADValue { get; set; }
    [field: SerializeField]
    public bool IsADPercentage { get; set; }
    [field: SerializeField]
    public float ASValue { get; set; }
    [field: SerializeField]
    public bool IsASPercentage { get; set; }
    [field: SerializeField]
    public float ARValue { get; set; }
    [field: SerializeField]
    public bool IsARPercentage { get; set; }
}
