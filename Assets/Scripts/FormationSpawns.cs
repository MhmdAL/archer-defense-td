using UnityEngine;

public class FormationSpawns : MonoBehaviour
{
    public GameObject GetSpawnPlatform(int index)
    {
        return transform.GetChild(index).gameObject;
    }
}