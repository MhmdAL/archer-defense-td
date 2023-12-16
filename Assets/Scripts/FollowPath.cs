using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMoving))]
public class FollowPath : MonoBehaviour
{
    public event Action OnPathCompleted;

    private IMoving targetMover;
    public PathData CurrentPath { get; private set; }
    public bool HasPath => CurrentPath != null;
    private int currentWaypointIndex = 0;

    private Vector3 currentTargetPosition;
    private Vector3 targetTargetPosition;

    private void Awake()
    {
        targetMover = GetComponent<IMoving>();
    }

    public void SetPath(PathData newPath, int currentIndex = 0, bool resetPosition = true)
    {
        if (newPath == null || newPath.Waypoints.Count == 0) return;

        CurrentPath = newPath;
        currentWaypointIndex = currentIndex;
        currentTargetPosition = GetRandomTargetPosition(currentIndex);
        targetTargetPosition = currentTargetPosition;

        if (resetPosition)
        {
            transform.root.position = currentTargetPosition;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentPath == null) return;

        currentTargetPosition = Vector3.Lerp(currentTargetPosition, targetTargetPosition, Time.deltaTime);

        float step = targetMover.MoveSpeed.Value * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.root.position, currentTargetPosition, step);

        transform.root.position = newPosition;

        if ((transform.root.position - currentTargetPosition).sqrMagnitude <= 3f)
        {
            HandleWaypointReached();
        }
    }

    private void HandleWaypointReached()
    {
        currentWaypointIndex++;
        targetTargetPosition = GetRandomTargetPosition(currentWaypointIndex);

        if (currentWaypointIndex >= CurrentPath.Waypoints.Count)
        {
            CurrentPath = null;

            OnPathCompleted?.Invoke();
        }
    }

    private Vector3 GetRandomTargetPosition(int waypointIndex)
    {
        return CurrentPath.Waypoints[waypointIndex] + (Vector3)UnityEngine.Random.insideUnitCircle * 1f;
    }
}