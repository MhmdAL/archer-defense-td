using UnityEngine;

public class PathGizmo : MonoBehaviour
{
    public Color pathColor;
    public Color sphereColor;

    private void OnDrawGizmos()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Gizmos.color = pathColor;

            Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);

            Gizmos.color = sphereColor;

            if (i == 1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.GetChild(i - 1).position, 1);
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.GetChild(transform.childCount - 1).position, 1);
    }
}
