using UnityEngine;

[ExecuteInEditMode]
public class PropRandomizer : MonoBehaviour
{
    public Transform UpperLeft;
    public Transform BottomRight;
    public GameObject Prefab;
    public int Count;

    [ContextMenu("Randomize Props")]
    public void RandomizeStuff()
    {
        var parent = new GameObject("PropParent");
        parent.transform.parent = this.transform;

        for(int i = 0; i < Count; i++)
        {
            var prop = Instantiate(Prefab, parent.transform);
            prop.transform.position = new Vector3(Random.Range(UpperLeft.position.x, BottomRight.position.x), Random.Range(BottomRight.position.y, UpperLeft.position.y), 0);
            var scale = Random.Range(0.8f, 1.2f);
            prop.transform.localScale = new Vector3(scale, scale, 1);

            var rotation = Random.Range(-15, 15);
            prop.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }

        print("Done");
    }
}
