using System;
using System.Collections.Generic;
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
    public BackgroundScaler Background { get; set; }

    public List<Tower> TowersInScene { get; private set; }
    public List<TowerBase> TowerBasesInScene { get; private set; }

    [field: SerializeField]
    private GameObject towerBaseHolder { get; set; }

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

    private void Awake()
    {
        _vs = FindObjectOfType<ValueStore>();

        TowersInScene = new List<Tower>();
        TowerBasesInScene = new List<TowerBase>();

        foreach (var item in towerBaseHolder.GetComponentsInChildren<TowerBase>())
        {
            TowerBasesInScene.Add(item);
        }

        _vs.userClickHandlerInstance.ObjectClicked += OnObjectClicked;
    }

    public void CreateTowerIfEnoughMoney(TowerBase tb)
    {
        if (_vs.Silver >= untrainedArcherPrefab.cost)
        {
            CreateTower(tb);
        }
    }

    private void CreateTower(TowerBase tb)
    {
        tb.SetState(TowerBaseState.NonClicked);

        var t = Instantiate(untrainedArcherPrefab, tb.originalPos, Quaternion.identity) as Tower;
        TowersInScene.Add(t);

        t.towerBase = tb;

        if (tb.gameObject.tag == "SuperBase")
        {
            t.AddModifier(new Modifier(superBaseAttackRangeModifier, Name.TowerBaseAttackRangeBuff,
                Type.ATTACK_RANGE, BonusOperation.Percentage), StackOperation.Additive, 1);

            t.buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_RANGE);
        }

        _vs.Silver -= t.cost;

        OnTowerDeployed(t);

        tb.gameObject.SetActive(false);
    }

    public void SellTower(Tower t)
    {
        // var towerToSell = _vs.lastClicked.GetComponent<Tower>();
        _vs.Silver += (int)(t.silverSpent * 0.6f);

        t.towerBase.gameObject.SetActive(true);

        TowersInScene.Remove(t);

        TowersInSceneChanged?.Invoke();

        Destroy(t.gameObject);

        TowerSold?.Invoke();
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

            newTower.modifiers.AddRange(t.modifiers);
            newTower.towerBase = t.towerBase;
            newTower.SkillPoints = t.SkillPoints;
            newTower.CurrentSkillLevel = t.CurrentSkillLevel;
            newTower.CurrentXP = t.CurrentXP;

            foreach (var item in t.buffIndicatorPanel.indicators)
            {
                newTower.buffIndicatorPanel.AddIndicator(item.type, item.cd);
            }

            if (newTower.towerBase.tag == "SuperBase")
            {
                newTower.AddModifier(new Modifier(superBaseAttackRangeModifier, Name.TowerBaseAttackRangeBuff,
                    Type.ATTACK_RANGE, BonusOperation.Percentage), StackOperation.Additive, 1);
            }

            newTower.silverSpent = t.cost;
            newTower.Upgrade();
            newTower.level += 1;

            _vs.Silver -= t.UpgradeCost;

            Destroy(t.gameObject);

            OnTowerSpecialized(newTower);
        }
    }

    public void ApplyDoubleShotEnhancement(Tower t)
    {
        var doubleShotStrat = ScriptableObject.CreateInstance<MultiShootNearestNMonstersTowerAttackStrategy>();
        doubleShotStrat.ShotCount = 2;
        doubleShotStrat.MultiShootSecondaries = true;

        t.TowerAttackStrategy = doubleShotStrat;
    }

    public void ApplyTowerEnhancement(Tower t, EnhancementType type)
    {
        Debug.Log(type);
        
        switch (type)
        {
            case EnhancementType.SlowOnAttack:
                t.ApplyEnhancement(new SlowOnAttackEnhancement());
                break;
            case EnhancementType.RampUp:
                t.ApplyEnhancement(new RampUpEnhancement());
                break;
            case EnhancementType.MultiShot:
                t.ApplyEnhancement(new MultiShotEnhancement());
                break;
        }
    }

    private void OnTowerSpecialized(Tower tower)
    {
        tower.Focus();

        TowerSpecialised?.Invoke(tower);
    }

    private void OnTowerDeployed(Tower tower)
    {
        tower.Focus();

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
        foreach (var t in TowersInScene)
        {
            t.UnFocus();
        }

        foreach (var tb in TowerBasesInScene)
        {
            tb.UnFocus();
        }

        tower.Focus();
    }

    public void OnTowerBaseClicked(TowerBase towerBase)
    {
        foreach (var t in TowersInScene)
        {
            t.UnFocus();
        }

        foreach (var tb in TowerBasesInScene)
        {
            tb.UnFocus();
        }

        towerBase.Focus();
    }

    public void OnBackgroundClicked()
    {
        foreach (var t in TowersInScene)
        {
            t.UnFocus();
        }

        foreach (var tb in TowerBasesInScene)
        {
            tb.UnFocus();
        }
    }

    private void OnDestroy()
    {
        _vs.userClickHandlerInstance.ObjectClicked -= OnObjectClicked;
    }
}

public enum TowerSkill
{
    AttackDamage,
    AttackSpeed,
    AttackRange
}