using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerSpecialtyEnhancementData : ScriptableObject
{
    [field: SerializeField]
    public List<int> EnhancementSilverRequirementsByLevel { get; set; }

    [field: SerializeField]
    public List<int> EnhancementSkillRequirementsByLevel { get; set; }

    [field: SerializeField]
    public List<TowerSpecialtyEnhancements> TowerSpecialtyEnhancements { get; set; }
}

[System.Serializable]
public class TowerSpecialtyEnhancements
{
    public ArcherType TowerSpecialty;

    public List<EnhancementTypesBySpecializationLevel> TowerEnhancementsBySpecializationLevel;
}

[System.Serializable]
public class EnhancementTypesBySpecializationLevel
{
    public int SpecializationLevel;

    public List<EnhancementType> EnhancementTypes;
}