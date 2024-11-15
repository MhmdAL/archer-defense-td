﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using UnityTimer;
using System;

public class AbilityArtillery : Ability<HorseRaidAbilityData>, IShooting
{
    public GameObject ArrowPrefab;
    public Transform StartPosition;
    public int ArrowCount;
    public float DamagePerArrow;
    public float ArrowTravelDuration;
    public float ArrowGravity;
    public float CooldownPerWave;
    public float ArrowSpread;
    public MinMaxCurve WaveCount;

    public GameObject ArtilleryPanel;
    public GameObject ArtilleryIndicator;

    private const float ArtilleryBaseDamage = 15;

    public List<TargetHitEffect> OnHitEffects { get; set; }

    protected override bool IsReady()
    {
        return CooldownFinished() && MonstersExist();
    }

    private bool MonstersExist() => vs.monsterManagerInstance.MonstersInScene.Count > 0;

    public void ActivateArtillery()
    {
        ArtilleryPanel.SetActive(false);
        ArtilleryIndicator.SetActive(true);

        var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0;

        ArtilleryIndicator.transform.position = targetPos;
        ArtilleryIndicator.transform.localScale = new Vector3(2 * ArrowSpread, 2 * ArrowSpread, 1);

        OnAbilityActivated();
        
        StartCoroutine(FireArtillery(targetPos));
    }

    public override void Execute()
    {
        ArtilleryPanel.SetActive(true);

        Debug.Log("Activating artillery");
    }

    private IEnumerator FireArtillery(Vector3 targetPos)
    {
        int arrowCount = (int)(SaveData.baseUpgradeValues[UpgradeType.ArtilleryArrowCount] +
            SaveData.GetUpgrade(UpgradeType.ArtilleryArrowCount)?.CurrentValue ?? 0);

        var waves = Mathf.Min(UnityEngine.Random.Range((int)WaveCount.constantMin, (int)WaveCount.constantMax + 1), ArrowCount);
        var arrowPerWave = ArrowCount / waves;
        var remainder = ArrowCount % waves;

        this.AttachTimer(ArrowTravelDuration + (waves - 1) * CooldownPerWave, (t) => ArtilleryIndicator.SetActive(false));

        for (int i = 0; i < waves; i++)
        {
            var arrowsToSpawn = i == waves - 1 ? arrowPerWave + remainder : arrowPerWave;

            for (int j = 0; j < arrowsToSpawn; j++)
            {
                SpawnArtilleryArrow(targetPos);
            }

            yield return new WaitForSeconds(CooldownPerWave + UnityEngine.Random.Range(0, 0.2f));
        }
    }

    private void SpawnArtilleryArrow(Vector3 target)
    {
        var arrow = Instantiate(ArrowPrefab, StartPosition.position, Quaternion.identity);
        var p = arrow.GetComponentInChildren<Projectile>();

        p.Owner = this;
        p.Damage = DamagePerArrow;
        p.StartPosition = StartPosition.position;
        p.TargetPosition = target + (Vector3)UnityEngine.Random.insideUnitCircle * ArrowSpread;
        p.Duration = ArrowTravelDuration;
        p.Gravity = ArrowGravity;
    }

    public void OnTargetHit(Vector3 targetPosition, List<IProjectileTarget> unitsHit, Projectile p, int shotNumber)
    {
        foreach (var unit in unitsHit)
        {
            if (unit is Unit u)
            {
                u.Damage(p.Damage, 0, DamageSource.Normal, this, null);
            }
        }
    }
}
