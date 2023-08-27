using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public float SpawningInterval = 10;

    public List<GameObject> PotionPrefabs;

    public GameObject PotionSpawningBounds;

    private List<Collider2D> _spawningAreas;
    private Bounds _spawningBounds;

    private void Awake()
    {
        AdjustSpawningBounds();

        StartCoroutine(Spawn());
    }

    private void AdjustSpawningBounds()
    {
        _spawningAreas = new List<Collider2D>();

        foreach (Transform area in PotionSpawningBounds.transform)
        {
            _spawningAreas.Add(area.GetComponent<Collider2D>());
        }

        _spawningBounds = new Bounds();

        foreach (var area in _spawningAreas)
        {
            _spawningBounds.Encapsulate(area.bounds);
        }
    }

    public void Reset(GameObject potionSpawningBounds)
    {
        StopAllCoroutines();

        PotionSpawningBounds = potionSpawningBounds;

        AdjustSpawningBounds();

        StartCoroutine(Spawn());
    }

    public void SetActive(bool active)
    {
        if (!active)
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(10);

        while (true)
        {
            SpawnRandomPotion();

            yield return new WaitForSeconds(SpawningInterval);
        }
    }

    private void SpawnRandomPotion()
    {
        var randomPointInBounds = RandomBetweenBounds(_spawningBounds);

        var withinBounds = _spawningAreas.Any(x =>
        {
            var minX = x.bounds.center.x - x.bounds.extents.x;
            var minY = x.bounds.center.y - x.bounds.extents.y;
            var maxX = x.bounds.center.x + x.bounds.extents.x;
            var maxY = x.bounds.center.y + x.bounds.extents.y;

            return minX <= randomPointInBounds.x && randomPointInBounds.x <= maxX && minY <= randomPointInBounds.y && randomPointInBounds.y <= maxY;
        });

        if (withinBounds)
        {
            Debug.Log("Spawning");

            var prefabIdx = Random.Range(0, PotionPrefabs.Count);

            Instantiate(PotionPrefabs[prefabIdx], randomPointInBounds, Quaternion.identity);
        }
    }

    private Vector3 RandomBetweenBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x),
            Random.Range(bounds.center.y - bounds.extents.y, bounds.center.y + bounds.extents.y),
            0);
    }
}