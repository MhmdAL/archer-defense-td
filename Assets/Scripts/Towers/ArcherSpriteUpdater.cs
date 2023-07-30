using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSpriteUpdater : MonoBehaviour
{
    public ArcherSkillSpriteData SpriteData;

    public Tower Owner;

    public ParticleSystem SkillAvailableParticles;

    public SpriteRenderer Hat;
    public SpriteRenderer Head;
    public SpriteRenderer Facial;

    private void Start()
    {
        Owner.SkillUpgraded += UpdateVisuals;
        Owner.SkillPointsChanged += OnSkillpointsChanged;

        UpdateVisuals();
    }

    private void OnSkillpointsChanged(Tower t)
    {
        SkillAvailableParticles.gameObject.SetActive(t.SkillPoints > 0);
    }

    private void UpdateVisuals()
    {
        Hat.sprite = SpriteData.ARSkillHats[Owner.ARSkill.CurrentLevel];
        Head.sprite = SpriteData.ADSkillExpressions[Owner.ADSkill.CurrentLevel];
        Facial.sprite = SpriteData.ASSkillFacials[Owner.ASSkill.CurrentLevel];
    }

    private void OnDestroy()
    {
        Owner.SkillUpgraded -= UpdateVisuals;
        Owner.SkillPointsChanged -= OnSkillpointsChanged;
    }
}
