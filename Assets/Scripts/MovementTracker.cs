using System;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    public Action<Vector3, Vector3> MovementChanged;

    public Vector3 LastPosition { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }

    private Vector3 prevPos;

    private Vector3 lastVelocity;

    private void FixedUpdate()
    {
        if (Time.timeScale == 0 || Time.deltaTime == 0)
            return;

        CurrentVelocity = (transform.position - prevPos) / Time.deltaTime;
        prevPos = transform.position;

        if (lastVelocity != CurrentVelocity)
        {
            MovementChanged?.Invoke(transform.position - LastPosition, CurrentVelocity);
        }

        LastPosition = transform.position;
        lastVelocity = CurrentVelocity;
    }
}