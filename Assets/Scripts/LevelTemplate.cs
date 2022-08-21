using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
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

    private void OnDrawGizmos()
    {
        Paths = PathsRoot.GetComponentsInChildren<Path>().ToList();

        if (Paths == null || !Paths.Any())
        {
            return;
        }

        foreach (var path in Paths)
        {
            for (int i = 0; i < path.transform.childCount; i++)
            {
                if (i < path.transform.childCount - 1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(path.transform.GetChild(i).position, path.transform.GetChild(i + 1).position);
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(path.transform.GetChild(i).position, 1);
            }
        }
        Gizmos.color = Color.white;
    }
}
