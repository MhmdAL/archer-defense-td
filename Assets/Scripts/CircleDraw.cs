using UnityEngine;
using System.Collections;

public class CircleDraw : MonoBehaviour
{
    float theta_scale = 0.01f;        //Set lower to add more points
    int size; //Total number of points in circle
    public float radius;
    LineRenderer lineRenderer;
    public Color c = Color.red;
    public Material m;

    void Start()
    {
        float sizeValue = (2.0f * Mathf.PI) / theta_scale;
        size = (int)sizeValue;
        size++;
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(m.shader);
        lineRenderer.SetWidth(0.05f, 0.05f); //thickness of line
        lineRenderer.SetVertexCount(size);
        Debug.Log(size);
    }

    void Update()
    {
        lineRenderer.SetColors(c, c);
        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < size; i++)
        {
            theta += (theta_scale);
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            x += gameObject.transform.position.x;
            y += gameObject.transform.position.y;
            pos = new Vector3(x, y, -1);
            lineRenderer.SetPosition(i, pos);
        }
    }
}