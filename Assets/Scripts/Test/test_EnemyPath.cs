using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_EnemyPath : MonoBehaviour
{
    public Monster Enemy;
    public Path Path;

    void Start()
    {
        Enemy.SetPath(Path);
    }
}
