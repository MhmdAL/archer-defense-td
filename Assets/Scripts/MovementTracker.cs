using System;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    public Action<Vector3, Vector3> MovementChanged;

    public Vector3 LastPosition { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }

    private float interval = 0.25f;
    private float timer;
    private Vector3 prevPos;

    private Vector3 lastVelocity;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            CurrentVelocity = (transform.position - prevPos) / interval;
            prevPos = transform.position;
        }

        if (lastVelocity != CurrentVelocity)
        {
            MovementChanged?.Invoke(transform.position - LastPosition, CurrentVelocity);
        }

        LastPosition = transform.position;
        lastVelocity = CurrentVelocity;
    }
}