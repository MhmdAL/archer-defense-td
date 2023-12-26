using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using UnityTimer;

public enum HorseRaidState
{
    Idle,
    Setup,
}

public class AbilityHorseRaid : Ability
{
    public GameObject HorseRaidScreenIndicator;
    public GameObject HorseRaidStartIndicatorPrefab;
    public GameObject HorseRaidEndIndicatorPrefab;
    public GameObject HorseRaiderPrefab;
    public int HorseRaiderCount = 3;
    public Transform HorseRaiderSpawnPosition;

    private HorseRaidState _state;

    private Vector2? _raidStartPosition;
    private Vector2? _raidEndPosition;

    private GameObject _raidStartIndicator = null;
    private GameObject _raidEndIndicator = null;


    public override void Execute()
    {
        _state = HorseRaidState.Setup;
        HorseRaidScreenIndicator.SetActive(true);

        Debug.Log("Activating horse raid");
    }

    protected override void Update()
    {
        base.Update();

        if (_state == HorseRaidState.Setup && Input.GetMouseButtonDown(0))
        {
            if (_raidStartPosition == null)
            {
                if(_raidStartIndicator != null)
                {
                    Destroy(_raidStartIndicator);
                    _raidStartIndicator = null;
                }

                if(_raidEndIndicator != null)
                {
                    Destroy(_raidEndIndicator);
                    _raidEndIndicator = null;
                }

                _raidStartPosition = Input.mousePosition;

                _raidStartIndicator = Instantiate(HorseRaidStartIndicatorPrefab, _raidStartPosition.Value.ToWorldPosition(Camera.main), Quaternion.identity);
            }
            else if (_raidEndPosition == null)
            {
                _raidEndPosition = Input.mousePosition;

                _raidEndIndicator = Instantiate(HorseRaidEndIndicatorPrefab, _raidEndPosition.Value.ToWorldPosition(Camera.main), Quaternion.identity);

                StartRaid();
            }
        }
    }

    private void StartRaid()
    {
        HorseRaidScreenIndicator.SetActive(false);

        Debug.Log("raid starting");
        _state = HorseRaidState.Idle;

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

                Destroy(_raidStartIndicator, 2f);
                Destroy(_raidEndIndicator, 2f);

                _raidStartIndicator = null;
                _raidEndIndicator = null;
            });
        }
    }

    protected override bool IsReady()
    {
        return CooldownFinished();
    }
}
