using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;

public class HorseRaiderV2 : MonoBehaviour, IMoving
{
    public float Speed;

    private Action RaidEndCallback;

    public Stat MoveSpeed { get; set; }

    private FollowPath followPath;

    private Vector3? Destination;

    [SerializeField]
    private Animator horseAnimator;

    private MovementTracker movementTracker;

    private void Awake()
    {
        MoveSpeed = new Stat(Type.MOVEMENT_SPEED, Speed);

        followPath = GetComponent<FollowPath>();

        followPath.OnPathCompleted += RaidEnded;

        movementTracker = GetComponent<MovementTracker>();

        movementTracker.MovementChanged += OnMovementChanged;
    }

    private void Update()
    {
        if (Destination != null)
        {
            var dir = Destination.Value - transform.root.position;

            if (dir.sqrMagnitude < 5f)
            {
                Destination = null;

                followPath.enabled = false;

                this.AttachTimer(3f, PatrolFinished);
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            StartPatrol(Input.mousePosition.ToWorldPosition(Camera.main));
        }
    }

    private void OnMovementChanged(Vector3 delta, Vector3 currentVelocity)
    {
        horseAnimator.SetFloat("walk_speed", Mathf.Abs(currentVelocity.x) * 0.1f);

        UpdateDirection(delta);
    }

    private void UpdateDirection(Vector3 diff)
    {
        if (diff.x < 0)
        {
            // Moving left
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if (diff.x >= 0)
        {
            // Moving right
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    public void StartPatrol(Vector3 destination)
    {
        var level = FindObjectOfType<LevelTemplate>();

        var path = FindNearestPath(destination, level.Paths);

        var reversedPath = path.PathData.ReversePath();

        followPath.SetPath(reversedPath, 0, false);

        destination.z = 0;
        Destination = reversedPath.GetNearestWaypoint(destination).Item1;
    }

    private void PatrolFinished(Timer t)
    {
        var reversedPath = followPath.CurrentPath.ReversePath();

        var (_, nextPoint) = reversedPath.GetNearestWaypoint(transform.root.position);

        if (reversedPath.Waypoints.Count > nextPoint + 1)
        {
            nextPoint++;
        }

        followPath.SetPath(reversedPath, nextPoint, false);

        followPath.enabled = true;
    }

    public Path FindNearestPath(Vector3 point, List<Path> paths)
    {
        Path nearestPath = null;
        float shortestDistance = float.MaxValue;

        foreach (var path in paths)
        {
            foreach (var waypoint in path.PathData.Waypoints)
            {
                float distance = Vector3.Distance(point, waypoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPath = path;
                }
            }
        }

        return nearestPath;
    }

    public HorseRaiderV2 OnRaidFinished(Action endCallback)
    {
        RaidEndCallback = endCallback;
        return this;
    }

    private void RaidEnded()
    {
        RaidEndCallback?.Invoke();
    }

    public void OnMovementStarted()
    {
        // throw new NotImplementedException();
    }

    public void OnMovementEnded()
    {
        // throw new NotImplementedException();
    }
}