using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using UnityTimer;

public enum HorseRaidState
{
    Idle,
    SetupPhase,
    Commencing,
}

public class AbilityHorseRaid : Ability
{
    public GameObject HorseRaiderPrefab;
    public int HorseRaiderCount = 3;
    public Transform HorseRaiderSpawnPosition;

    private HorseRaidState _state;

    private Vector2? _raidStartPosition;
    private Vector2? _raidEndPosition;


    public override void Activate()
    {
        _state = HorseRaidState.SetupPhase;
    }

    protected override void Update()
    {
        base.Update();

        if (_state == HorseRaidState.SetupPhase && Input.GetMouseButtonDown(0))
        {
            if (_raidStartPosition == null)
            {
                _raidStartPosition = Input.mousePosition;
            }
            else if (_raidEndPosition == null)
            {
                _raidEndPosition = Input.mousePosition;

                StartRaid();
            }
        }
    }

    private void StartRaid()
    {
        Debug.Log("raid starting");
        _state = HorseRaidState.Commencing;

        var raidStartPos = _raidStartPosition.Value.ToWorldPosition(Camera.main);
        var raidEndPos = _raidEndPosition.Value.ToWorldPosition(Camera.main);

        for (int i = 0; i < HorseRaiderCount; i++)
        {
            var horseRaiderObj = Instantiate(HorseRaiderPrefab, HorseRaiderSpawnPosition);
            var horseRaiderComponent = horseRaiderObj.GetComponent<HorseRaider>();

            var path = new Waypoint
            {
                Position = HorseRaiderSpawnPosition.position + (Vector3)(Random.insideUnitCircle * 5f),
                NextWaypoint = new Waypoint
                {
                    Position = raidStartPos,
                    NextWaypoint = new Waypoint
                    {
                        Position = raidEndPos,
                        NextWaypoint = new Waypoint
                        {
                            Position = HorseRaiderSpawnPosition.position + (Vector3)(Random.insideUnitCircle * 5f)
                        }
                    }
                }
            };

            _raidStartPosition = null;
            _raidEndPosition = null;

            horseRaiderComponent.StartRaid(path).OnRaidFinished(() =>
            {
                Destroy(horseRaiderObj);
            });
        }
    }

    public override void UpdateReadiness()
    {
        if (CooldownTimer.GetTimeRemaining() <= 0)
        {
            SetReady(true);
        }
    }
}
