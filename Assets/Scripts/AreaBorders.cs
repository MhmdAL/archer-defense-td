using UnityEngine;

public class AreaBorders : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time * speed, 0f));
    }
}
