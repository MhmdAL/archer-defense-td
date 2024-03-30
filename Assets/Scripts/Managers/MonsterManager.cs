using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MonsterManager : MonoBehaviour
{

    public delegate void EnemyActionHandler(Monster m);
    public event EnemyActionHandler EnemySpawned;
    public event Action<Monster, DamageSource> EnemyDied;

    public float easyMultiplier = 1, intermediateMultiplier = 2f, expertMultiplier = 3f;

    public float CurrentMultiplier
    {
        get
        {
            return ValueStore.Instance.level.difficulty == LevelDifficulty.Easy ?
                easyMultiplier : ValueStore.Instance.level.difficulty == LevelDifficulty.Medium ? intermediateMultiplier : expertMultiplier;
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

    private void Start()
    {
        // monstersInScene = FindObjectsOfType<Monster>().ToList();
    }

    public void Reset()
    {
        MonstersInScene.Where(x => x != null).ToList().ForEach(x => Destroy(x.transform.root.gameObject));
        MonstersInScene.Clear();
    }

    public void OnEnemySpawned(Monster m)
    {
        MonstersInScene.Add(m);
        if (EnemySpawned != null)
        {
            EnemySpawned(m);
        }
    }

    public void OnEnemyDied(Monster m, DamageSource source)
    {
        MonstersInScene.Remove(m);
        if (EnemyDied != null)
        {
            EnemyDied(m, source);
        }
    }

    public Monster SpawnEnemy(GameObject prefab, int entrance, int exit)
    {
        // Spawn enemy
        GameObject m = (GameObject)Instantiate(prefab, new Vector3(1000, 1000, 0), Quaternion.identity);
        Monster mon = m.GetComponentInChildren<Monster>();

        // Set Path
        List<Path> relevantPaths;
        if (entrance != 0 && exit != 0)
        {
            relevantPaths = paths.Where(x => x.entrance == entrance && x.exit == exit).ToList();
        }
        else if (entrance == 0 && exit != 0)
        {
            relevantPaths = paths.Where(x => x.exit == exit).ToList();
        }
        else if (entrance != 0 && exit == 0)
        {
            relevantPaths = paths.Where(x => x.entrance == entrance).ToList();
        }
        else
        {
            relevantPaths = paths.ToList();
        }
        int random = UnityEngine.Random.Range(1, relevantPaths.Count + 1);

        mon.SetPath(relevantPaths[random - 1]);

        // Raise enemyspawned event
        OnEnemySpawned(mon);

        mon.OnDeath += (unit, ds) => OnEnemyDied(unit as Monster, ds);

        return mon;
    }

    public static bool DoesKill(Monster m, float damage, float armorPen)
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
