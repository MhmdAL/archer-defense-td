using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMoving))]
public class FollowPath : MonoBehaviour
{
    public event Action OnPathCompleted;
    public event Action<Vector2> TargetChanged;

    private IMoving targetMover;
    public PathData CurrentPath { get; private set; }
    public bool HasPath => CurrentPath != null;
    private int currentWaypointIndex = 0;

    private Vector3 currentTargetPosition;

    private void Awake()
    {
        targetMover = GetComponent<IMoving>();
    }

    public void SetPath(PathData newPath, int currentIndex = 0, bool resetPosition = true)
    {
        if (newPath == null || newPath.Waypoints.Count == 0) return;

        CurrentPath = newPath;
        currentWaypointIndex = currentIndex;
        
        UpdateTarget(currentWaypointIndex);

        if (resetPosition)
        {
            transform.root.position = currentTargetPosition;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentPath == null) return;

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

        if (currentWaypointIndex >= CurrentPath.Waypoints.Count)
        {
            CurrentPath = null;

            OnPathCompleted?.Invoke();
        }
        else
        {
            UpdateTarget(currentWaypointIndex);
        }
    }

    private void UpdateTarget(int currentIndex)
    {
        currentTargetPosition = GetRandomTargetPosition(currentIndex);

        TargetChanged?.Invoke(currentTargetPosition - transform.root.position);
    }

    private Vector3 GetRandomTargetPosition(int waypointIndex)
    {
        return CurrentPath.Waypoints[waypointIndex] + (Vector3)UnityEngine.Random.insideUnitCircle * 1f;
    }
}