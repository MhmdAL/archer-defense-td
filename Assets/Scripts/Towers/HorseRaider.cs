using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRaider : MonoBehaviour
{
    private Waypoint _currentTarget;
    private Action RaidEndCallback;

    public HorseRaider StartRaid(Waypoint path)
    {
        _currentTarget = path;
        return this;
    }

    public HorseRaider OnRaidFinished(Action endCallback)
    {
        RaidEndCallback = endCallback;
        return this;
    }

    private void Update()
    {
        if(_currentTarget != null)
        {
            var dir = _currentTarget.Position - transform.position;

            if(dir.magnitude <= 0.5f)
            {
                _currentTarget = _currentTarget.NextWaypoint;

                if(_currentTarget == null)
                {
                    RaidEndCallback?.Invoke();
                }

                return;
            }

            transform.position += dir.normalized * 10 * Time.deltaTime;
        }
    }
}

public class Waypoint
{
    public Vector3 Position;
    public Waypoint NextWaypoint;
}