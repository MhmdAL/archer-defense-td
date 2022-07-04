/*using UnityEngine;
using System.Collections;

public class ThrowSimulation : MonoBehaviour {

    public Transform Target;
    public float firingAngle = 1f;
    public float gravity = 2000f;

    public Transform Projectile;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
        /*float distance = Vector3.Distance(closestObject.transform.position, gameObject.transform.position);
        float distancediv = distance / 180;
        if (closestObject.transform.position.x < gameObject.transform.position.x && closestObject.transform.position.y < gameObject.transform.position.y)
        {
            if (closestObject.GetComponent<Monster>().currentGround.tag != VerticalGroundUp")
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.x - gameObject.transform.position.x) <= distancediv * i)
                    {
                        degree = 90 - i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.y - gameObject.transform.position.y) <= distancediv * i)
                    {
                        degree = i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
        }
        else if (closestObject.transform.position.x > gameObject.transform.position.x && closestObject.transform.position.y < gameObject.transform.position.y)
        {
            if (closestObject.GetComponent<Monster>().currentGround.tag != "VerticalGroundUp")
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.x - gameObject.transform.position.x) <= distancediv * i)
                    {
                        degree = 90 + i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.y - gameObject.transform.position.y) <= distancediv * i)
                    {
                        degree = i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
        }
        else if (closestObject.transform.position.x > gameObject.transform.position.x && closestObject.transform.position.y > gameObject.transform.position.y)
        {
            if (closestObject.GetComponent<Monster>().currentGround.tag != "VerticalGroundUp")
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.x - gameObject.transform.position.x) <= distancediv * i)
                    {
                        degree = 270 - i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.y - gameObject.transform.position.y) <= distancediv * i)
                    {
                        degree = i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
        }
        else if (closestObject.transform.position.x < gameObject.transform.position.x && closestObject.transform.position.y > gameObject.transform.position.y)
        {
            if (closestObject.GetComponent<Monster>().currentGround.tag != "VerticalGroundUp")
            {
                for (int i = 1; i < 360; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.x - gameObject.transform.position.x) <= distancediv * i)
                    {
                        degree = 270 + i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 1; i < 181; i++)
                {
                    if (Mathf.Abs(closestObject.transform.position.y - gameObject.transform.position.y) <= distancediv * i)
                    {
                        degree = i * 0.5f;
                        setDegree(degree);
                        return;
                    }
                }
            }
        }
    }

    void Start()
    {

        StartCoroutine(SimulateProjectile());
    }
    void Update()
    {
        float step = 15 * Time.deltaTime;
       // Projectile.position = Vector3.MoveTowards(Projectile.position, Target.position, step);
        //Target.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 1, 0);
    }

    IEnumerator SimulateProjectile()
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        //Projectile.position = myTransform.position + new Vector3(0, 5.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(Projectile.position, Target.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2   * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(  Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(  Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        //Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            //Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
    }
}*/