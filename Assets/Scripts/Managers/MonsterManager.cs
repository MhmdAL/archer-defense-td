using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MonsterManager : MonoBehaviour
{

    public delegate void EnemyActionHandler(Monster m);
    public event EnemyActionHandler EnemySpawned;
    public event EnemyActionHandler EnemyDied;

    public float easyMultiplier = 1, intermediateMultiplier = 2f, expertMultiplier = 3f;

    public float CurrentMultiplier
    {
        get
        {
            return ValueStore.sharedInstance.level.difficulty == LevelDifficulty.Easy ?
                easyMultiplier : ValueStore.sharedInstance.level.difficulty == LevelDifficulty.Medium ? intermediateMultiplier : expertMultiplier;
        }
    }

    public GameObject deathParticlePrefab;

    public Path[] paths;

    private List<Monster> monstersInScene = new List<Monster>();
    public List<Monster> MonstersInScene
    {
        get
        {
            return monstersInScene;
        }
        set
        {
            monstersInScene = value;
        }
    }

    [Header("Star rating Options")]
    public float maximumArmor, maximumHP;

    public void OnEnemySpawned(Monster m)
    {
        MonstersInScene.Add(m);
        if (EnemySpawned != null)
        {
            EnemySpawned(m);
        }
    }

    public void OnEnemyDied(Monster m)
    {
        MonstersInScene.Remove(m);
        if (EnemyDied != null)
        {
            EnemyDied(m);
        }
    }

    public void SpawnEnemy(GameObject prefab, int entrance, int exit)
    {
        // Spawn enemy
        GameObject m = (GameObject)Instantiate(prefab, new Vector3(1000, 1000, 0), Quaternion.identity);
        Monster mon = m.GetComponentInChildren<Monster>();
        // Set Path
        mon.SetPath(entrance, exit);
        // Raise enemyspawned event
        OnEnemySpawned(mon);
    }

    public bool DoesKill(Monster m, float damage, float armorPen)
    {
        float modifiedDamage = damage * m.DamageModifier.Value;
        if (m != null)
        {
            if (m.CurrentHP > 0)
            {
                if (m.Armor.Value - armorPen > 0)
                {
                    if (m.CurrentHP - (float)(modifiedDamage * (50 / (50 + (m.Armor.Value - (armorPen * 2))))) <= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (m.CurrentHP - modifiedDamage <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
}
