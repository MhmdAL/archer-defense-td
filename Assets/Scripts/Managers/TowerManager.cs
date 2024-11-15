using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public event Action TowersInSceneChanged;
    public event Action<Tower> TowerDeployed;
    public event Action TowerSold;
    public event Action<Tower> TowerUpgraded;
    public event Action<Tower> TowerSpecialised;
    public event Action<Tower> TowerClicked;
    public event Action<TowerBase> TowerBaseClicked;

    [field: SerializeField]
    public TowerSpecialtyEnhancementData TowerSpecialtyEnhancementData { get; set; }

    public List<Tower> TowersInScene { get; private set; }
    public List<TowerBase> TowerBasesInScene { get; set; }

    public GameObject TowerCreateParticles;

    [field: SerializeField]
    private Tower untrainedArcherPrefab { get; set; }

    [field: SerializeField]
    private Tower rapidArcherPrefab { get; set; }

    [field: SerializeField]
    private Tower longArcherPrefab { get; set; }

    [field: SerializeField]
    private Tower utilityArcherPrefab { get; set; }

    [field: SerializeField]
    private float superBaseAttackRangeModifier { get; set; }

    private ValueStore _vs;

    private List<Tower> _spawnedFormations = new List<Tower>();

    private void Awake()
    {
        _vs = FindObjectOfType<ValueStore>();

        TowersInScene = new List<Tower>();
        TowerBasesInScene = new List<TowerBase>();

        _vs.userClickHandlerInstance.ObjectClicked += OnObjectClicked;
        _vs.WaveSpawner.WaveEnded += OnWaveEnded;
        _vs.WaveSpawner.WaveReset += OnWaveReset;
        _vs.LevelStarted += OnLevelStarted;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && FindObjectOfType<BackgroundScaler>().HasFocus)
        {
            CreateTower(untrainedArcherPrefab, _vs.CurrentLevel.FormationSpawns.GetSpawnPlatform(0).transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 10);
        }
    }

    public void Reset()
    {
        TowersInScene.Where(x => x != null).ToList().ForEach(x => Destroy(x.gameObject));

        TowerBasesInScene.Clear();
        TowersInScene.Clear();

        _spawnedFormations.Clear();
    }

    public void CreateTowerIfEnoughMoney(TowerBase tb)
    {
        if (_vs.Silver >= untrainedArcherPrefab.cost)
        {
            tb.SetState(TowerBaseState.NonClicked);

            var tower = CreateTower(untrainedArcherPrefab, tb.originalPos);

            tower.TowerBase = tb;

            tb.gameObject.SetActive(false);
        }
    }

    private void OnLevelStarted()
    {
        foreach (var item in _vs.CurrentLevel.LevelData.StartingFormations)
        {
            var startingFormation = item.FormationPrefab;
            var startingFormationSpawnIndex = item.FormationSpawnIndex;

            if (startingFormation != null)
            {
                SpawnUnitFormation(startingFormation, startingFormationSpawnIndex);
            }
        }
    }

    private List<Tower> SpawnUnitFormation(GameObject formation, int spawnIndex)
    {
        var spawnPos = _vs.CurrentLevel.FormationSpawns.GetSpawnPlatform(spawnIndex);

        var spawnedTowers = new List<Tower>();

        foreach (var tower in formation.GetComponentsInChildren<Tower>())
        {
            spawnedTowers.Add(CreateTower(tower, spawnPos.transform.position + tower.transform.localPosition));
        }

        return spawnedTowers;
    }

    private void OnWaveEnded(int wave)
    {
        foreach (var item in _vs.WaveSpawner.LevelData.Waves[wave - 1].WaveRewards)
        {
            if (item != null)
            {
                _spawnedFormations.AddRange(SpawnUnitFormation(item.FormationPrefab, item.FormationSpawnIndex));
            }
        }
    }

    private void OnWaveReset(int wave)
    {
        foreach (var item in _spawnedFormations.ToList())
        {
            Destroy(item.gameObject);
        }

        _spawnedFormations.Clear();

        for (int i = 0; i < wave - 1; i++)
        {
            foreach (var item in _vs.WaveSpawner.LevelData.Waves[i].WaveRewards)
            {
                if (item != null)
                {
                    _spawnedFormations.AddRange(SpawnUnitFormation(item.FormationPrefab, item.FormationSpawnIndex));
                }
            }
        }
    }

    private Tower CreateTower(Tower prefab, Vector3 position)
    {
        var t = Instantiate(prefab, position, Quaternion.identity) as Tower;
        TowersInScene.Add(t);

        // if (tb.gameObject.tag == "SuperBase")
        // {
        //     t.AR.Modify(superBaseAttackRangeModifier, BonusOperation.Percentage, Name.TowerBaseAttackRangeBuff.ToString());

        //     t.buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_RANGE);
        // }

        _vs.Silver -= t.cost;

        OnTowerDeployed(t);

        Instantiate(TowerCreateParticles, t.transform.position - Vector3.up * 2, Quaternion.identity);

        return t;
    }

    public void SellTower(Tower t)
    {
        _vs.Silver += (int)(t.silverSpent * 0.6f);

        t.TowerBase.gameObject.SetActive(true);

        TowersInScene.Remove(t);

        TowersInSceneChanged?.Invoke();

        Destroy(t.gameObject);

        TowerSold?.Invoke();
    }

    public (int silverCost, int skillLevelRequired) GetSpecializationRequirements(int specializationLevel)
    {
        var skillLevelRequirement = TowerSpecialtyEnhancementData.EnhancementSkillRequirementsByLevel;
        var silverCost = TowerSpecialtyEnhancementData.EnhancementSilverRequirementsByLevel;

        return (silverCost[specializationLevel], skillLevelRequirement[specializationLevel]);
    }

    public void ApplyEnhancement(Tower t, EnhancementType type)
    {
        var requirements = GetSpecializationRequirements(t.SpecializationLevel);

        if (_vs.Silver < requirements.silverCost || t.CurrentSkillLevel < requirements.skillLevelRequired)
        {
            return;
        }

        if (t.SpecializationLevel == 0) // Tower is not specialized
        {
            t.ArcherSpecialty = GetSpecialtyFromFirstEnhancement(type);
            t.SpecializationLevel = 1;
        }
        else
        {
            t.SpecializationLevel++;
        }

        switch (type)
        {
            case EnhancementType.SlowOnAttack:
                t.ApplyEnhancement(new SlowOnAttackEnhancement());
                break;
            case EnhancementType.RampUp:
                t.ApplyEnhancement(new RampUpEnhancement());
                break;
            case EnhancementType.MultiShot:
                t.ApplyEnhancement(new MultiShotEnhancement(new MultiShotEnhancementData { SecondaryTargetCount = 5 }));
                break;
            case EnhancementType.Headshot:
                t.ApplyEnhancement(new HeadshotEnhancement());
                break;
            case EnhancementType.Executioner:
                t.ApplyEnhancement(new ExecutionerEnhancement());
                break;
            case EnhancementType.Berzerk:
                t.ApplyEnhancement(new BerzerkEnhancement());
                break;
        }
    }

    public List<EnhancementType> GetNextPossibleEnhancements(Tower t)
    {
        var towerSpecialtyEnhancementData = TowerSpecialtyEnhancementData.TowerSpecialtyEnhancements
                                .FirstOrDefault(x => x.TowerSpecialty == t.ArcherSpecialty);

        if (towerSpecialtyEnhancementData == null)
        {
            var firstLevelEnhancements = TowerSpecialtyEnhancementData.TowerSpecialtyEnhancements
                                .SelectMany(x => x.TowerEnhancementsBySpecializationLevel)
                                .Where(x => x.SpecializationLevel == 0)
                                .SelectMany(x => x.EnhancementTypes)
                                .ToList();

            return firstLevelEnhancements;
        }

        return towerSpecialtyEnhancementData.TowerEnhancementsBySpecializationLevel
                                .FirstOrDefault(x => x.SpecializationLevel == t.SpecializationLevel)?.EnhancementTypes;
    }

    private ArcherType GetSpecialtyFromFirstEnhancement(EnhancementType enhancement)
    {
        var towerSpecialty = TowerSpecialtyEnhancementData.TowerSpecialtyEnhancements?.FirstOrDefault(x =>
        {
            var level0Enhancements = x.TowerEnhancementsBySpecializationLevel.FirstOrDefault(y => y.SpecializationLevel == 0);

            return level0Enhancements.EnhancementTypes.Contains(enhancement);
        }).TowerSpecialty;

        if (towerSpecialty == null || towerSpecialty == default)
        {
            return ArcherType.ClassicArcher;
        }

        return towerSpecialty.Value;
    }

    public void UpgradeTower(Tower t)
    {
        if (_vs.Silver >= t.UpgradeCost && Tower.CanUpgrade(t))
        {
            t.Upgrade();

            _vs.Silver -= t.UpgradeCost;

            t.level += 1;

            TowerUpgraded?.Invoke(t);
        }
    }

    public void SetSpeciality(Tower t, int archerID)
    {
        if (_vs.Silver >= t.UpgradeCost)
        {
            Tower newTower = null;
            if (archerID == 1)
            {
                newTower = Instantiate(rapidArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
            }
            else if (archerID == 2)
            {
                newTower = Instantiate(longArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
            }
            else if (archerID == 3)
            {
                newTower = Instantiate(utilityArcherPrefab, t.gameObject.transform.position, Quaternion.identity) as Tower;
            }

            TowersInScene.Remove(t);
            TowersInScene.Add(newTower);

            newTower.TowerBase = t.TowerBase;
            newTower.SkillPoints = t.SkillPoints;
            newTower.CurrentSkillLevel = t.CurrentSkillLevel;
            newTower.CurrentXP = t.CurrentXP;

            foreach (var item in t.buffIndicatorPanel.indicators)
            {
                newTower.buffIndicatorPanel.AddIndicator(item.type, item.cd);
            }

            if (newTower.TowerBase.tag == "SuperBase")
            {
                // newTower.AddModifier(new Modifier(superBaseAttackRangeModifier, Name.TowerBaseAttackRangeBuff,
                //     Type.ATTACK_RANGE, BonusType.Percentage), StackOperation.Additive, 1);
            }

            newTower.silverSpent = t.cost;
            newTower.Upgrade();
            newTower.level += 1;

            _vs.Silver -= t.UpgradeCost;

            Destroy(t.gameObject);

            OnTowerSpecialized(newTower);
        }
    }

    private void OnTowerSpecialized(Tower tower)
    {
        // tower.Focus();

        TowerSpecialised?.Invoke(tower);
    }

    private void OnTowerDeployed(Tower tower)
    {
        // tower.Focus();

        TowerDeployed?.Invoke(tower);
        TowersInSceneChanged?.Invoke();
    }

    private void OnObjectClicked(object obj)
    {
        if (obj is BackgroundScaler bg)
        {
            OnBackgroundClicked();
        }
        else if (obj is Tower t)
        {
            OnTowerClicked(t);
        }
        else if (obj is TowerBase tb)
        {
            OnTowerBaseClicked(tb);
        }
    }

    public void OnTowerClicked(Tower tower)
    {
        // foreach (var t in TowersInScene)
        // {
        //     t.UnFocus();
        // }

        // foreach (var tb in TowerBasesInScene)
        // {
        //     tb.UnFocus();
        // }

        // tower.Focus();
    }

    public void OnTowerBaseClicked(TowerBase towerBase)
    {
        // foreach (var t in TowersInScene)
        // {
        //     t.UnFocus();
        // }

        // foreach (var tb in TowerBasesInScene)
        // {
        //     tb.UnFocus();
        // }

        // towerBase.Focus();
    }

    public void OnBackgroundClicked()
    {
        // foreach (var t in TowersInScene)
        // {
        //     t.UnFocus();
        // }

        // foreach (var tb in TowerBasesInScene)
        // {
        //     tb.UnFocus();
        // }
    }

    private void OnDestroy()
    {
        _vs.userClickHandlerInstance.ObjectClicked -= OnObjectClicked;
        _vs.WaveSpawner.WaveStarted -= OnWaveEnded;
        _vs.LevelStarted -= OnLevelStarted;
    }
}

public class TowerSkill
{
    public TowerSkillType SkillType;
    public int CurrentLevel;
}

public enum TowerSkillType
{
    AttackDamage,
    AttackSpeed,
    AttackRange
}