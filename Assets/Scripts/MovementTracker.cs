using System;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    public Action<Vector3, Vector3> MovementChanged;

    public Vector3 LastPosition { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }

    private Vector3 lastVelocity;

    private void Update()
    {
        CurrentVelocity = (transform.position - LastPosition) / Time.deltaTime;

        if(lastVelocity != CurrentVelocity)
        {
            MovementChanged?.Invoke(transform.position - LastPosition, CurrentVelocity);
        }

        LastPosition = transform.position;
        lastVelocity = CurrentVelocity;
    }
}