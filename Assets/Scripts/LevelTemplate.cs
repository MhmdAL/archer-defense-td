using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelTemplate : MonoBehaviour
{
    public int LevelId;
    
    public LevelData LevelData;
    
    public GameObject TowerBasesRoot;
    public GameObject PathsRoot;

    public List<TowerBase> TowerBases { get; set; }
    public List<Path> Paths { get; set; }

    private void Awake()
    {
        TowerBases = TowerBasesRoot.GetComponentsInChildren<TowerBase>().ToList();
        Paths = PathsRoot.GetComponentsInChildren<Path>().ToList();
    }
}
