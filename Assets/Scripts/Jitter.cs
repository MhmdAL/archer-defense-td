using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jitter : MonoBehaviour
{
    private Vector3 initialPos;
    public float scale;
    public float speed;

    private Vector3 curOffset;
    private bool upDirection;

    void Awake()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        curOffset += new Vector3(0, upDirection ? speed : -speed, 0);

        if(curOffset.y < -10)
        {
            upDirection = true;
        }
        else if(curOffset.y > 10)
        {
            upDirection = false;
        }

        transform.position = initialPos + curOffset;
    }
}
