using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public Action TowersInSceneChanged { get; set; }
    public Action<Tower> TowerDeployed { get; set; }
    public Action TowerSold { get; set; }
    public Action<Tower> TowerUpgraded { get; set; }
    public Action<Tower> TowerSpecialised;

    public List<Tower> TowersInScene { get; private set; }
    public List<TowerBase> TowerBasesInScene { get; private set; }

    [field: SerializeField]
    public ArcherSpecialtyData ArcherSpecialtyData { get; private set; }

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

        TowerDeployed?.Invoke(t);
        TowersInSceneChanged?.Invoke();

        tb.gameObject.SetActive(false);
    }

    public void SellTower()
    {
        var towerToSell = _vs.lastClicked.GetComponent<Tower>();
        _vs.Silver += (int)(towerToSell.silverSpent * 0.6f);

        towerToSell.towerBase.gameObject.SetActive(true);

        TowersInScene.Remove(towerToSell);

        TowersInSceneChanged?.Invoke();

        Destroy(_vs.lastClicked);

        TowerSold?.Invoke();
    }

    public void UpgradeTower(Tower t)
    {
        int nextLvl = t.level + 1;

        if (_vs.Silver >= t.UpgradeCost && Tower.CanUpgrade(t))
        {
            t.Upgrade();

            _vs.Silver -= t.UpgradeCost;

            t.level += 1;

            TowerUpgraded?.Invoke(t);
        }
    }

    public void SetSpeciality(int archerID)
    {
        var t = _vs.lastClickedTower;
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

            _vs.specialtyMenu.SetActive(false);
            _vs.Silver -= t.UpgradeCost;

            _vs.lastClicked = newTower.gameObject;
            _vs.lastClickedTower = newTower;

            Destroy(t.gameObject);

            TowerSpecialised?.Invoke(newTower);
        }
    }

    public void UseSkillpoint(int skill)
    {
        var tower = _vs.lastClickedTower;

        tower.SkillPoints--;

        var upgradeValues = ArcherSpecialtyData.SpecialtyValues[tower.CurrentArcherSpecialtyLevel];

        tower.CurrentArcherSpecialtyLevel++;

        switch ((TowerSkill)skill)
        {
            case TowerSkill.AttackDamage:
                tower.AddModifier(new Modifier(upgradeValues.ADValue,
                 Name.ArcherSpecialtyADBuff1, Type.ATTACK_DAMAGE,
                 upgradeValues.IsADPercentage ? BonusOperation.Percentage : BonusOperation.Flat
                ), StackOperation.Additive, 1);
                break;
            case TowerSkill.AttackSpeed:
                tower.AddModifier(new Modifier(upgradeValues.ASValue,
                 Name.ArcherSpecialtyASBuff1, Type.ATTACK_SPEED,
                 upgradeValues.IsASPercentage ? BonusOperation.Percentage : BonusOperation.Flat
                ), StackOperation.Additive, 1);
                break;
            case TowerSkill.AttackRange:
                tower.AddModifier(new Modifier(upgradeValues.ARValue,
                 Name.ArcherSpecialtyARBuff1, Type.ATTACK_RANGE,
                 upgradeValues.IsARPercentage ? BonusOperation.Percentage : BonusOperation.Flat
                ), StackOperation.Additive, 1);
                break;
        }
    }

}

public enum TowerSkill
{
    AttackDamage,
    AttackSpeed,
    AttackRange
}