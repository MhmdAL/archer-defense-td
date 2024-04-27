using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;

public class HorseRaiderV2 : MonoBehaviour, IMoving, ICleanable
{
    public float Speed;

    public float LoiterDuration; // seconds

    public Action PatrolStarted;
    public Action RaidEndCallback;

    public Stat MoveSpeed { get; set; }

    private FollowPath followPath;

    [SerializeField]
    private Animator horseAnimator;

    public MovementTracker movementTracker;

    private bool _loiterFinished = false;

    private void Awake()
    {
        MoveSpeed = new Stat(Type.MOVEMENT_SPEED, Speed);

        followPath = GetComponent<FollowPath>();

        followPath.DestinationReached += OnPathFinished;
        followPath.TargetWaypointChanged += OnTargetChanged;

        movementTracker = GetComponent<MovementTracker>();

        movementTracker.MovementChanged += OnMovementChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // StartRaid(Input.mousePosition.ToWorldPosition(Camera.main));
        }
    }

    private void OnMovementChanged(Vector3 delta, Vector3 currentVelocity)
    {
        horseAnimator.SetFloat("walk_speed", currentVelocity.magnitude);

        UpdateDirection(delta);
    }

    private void OnTargetChanged(Vector2 direction)
    {
        UpdateDirection(direction);
    }

    private void UpdateDirection(Vector3 diff)
    {
        if (diff.x == 0)
        {

        }
        else if (diff.x < 0)
        {
            // Moving left
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if (diff.x > 0)
        {
            // Moving right
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    public void StartRaid(PathData path)
    {
        followPath.SetPath(path, 0, false);
    }

    private void OnPatrolStarted()
    {
        followPath.enabled = false;

        PatrolStarted?.Invoke();

        this.AttachTimer(LoiterDuration, OnPatrolFinished);
    }

    private void OnPatrolFinished(Timer t)
    {
        _loiterFinished = true;

        var reversedPath = followPath.CurrentPath.ReversePath();

        // var (_, nextPoint) = reversedPath.GetNearestWaypoint(transform.root.position);

        // if (reversedPath.Waypoints.Count > nextPoint + 1)
        // {
        //     nextPoint++;
        // }

        followPath.SetPath(reversedPath, 0, false);

        followPath.enabled = true;

        RaidEndCallback?.Invoke();
    }

    private void OnPathFinished()
    {
        if (_loiterFinished)
        {
            Destroy(gameObject);
        }
        else
        {
            OnPatrolStarted();
        }
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

    public void OnMovementStarted()
    {
        // throw new NotImplementedException();
    }

    public void OnMovementEnded()
    {
        // throw new NotImplementedException();
    }

    public void CleanUp()
    {
        Destroy(gameObject);
    }
}