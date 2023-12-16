using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHorseRaidV2 : Ability
{
    public Action RaidStarted;

    public GameObject HorseRaidScreenIndicator;
    public GameObject HorseRaidStartIndicatorPrefab;
    public GameObject HorseRaidEndIndicatorPrefab;
    public GameObject HorseRaiderPrefab;
    public int HorseRaiderCount = 3;
    public float HorseRaiderSpawnDelay = 1;
    public Transform HorseRaiderSpawnPosition;
    public float horseRaidPositionRandomFactor = 1f;

    private HorseRaidState _state;

    private Vector2? _raidStartPosition;
    private Vector2? _raidEndPosition;

    private GameObject _raidStartIndicator = null;
    public GameObject raidEndIndicator;

    public CustomStandaloneInputModule inputModule;

    private CursorManager _cursorManager;

    protected override void Awake()
    {
        base.Awake();

        _cursorManager = FindObjectOfType<CursorManager>();
    }

    protected override bool IsReady()
    {
        return CooldownFinished();
    }

    public override void Activate()
    {
        _state = HorseRaidState.Setup;
        HorseRaidScreenIndicator.SetActive(true);

        _cursorManager.UseCursor(CursorType.PendingRaid);

        Debug.Log("Activating horse raid");
    }

    protected override void Update()
    {
        base.Update();

        if (_state == HorseRaidState.Setup)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitRaidSetup();

                return;
            }

            _cursorManager.UpdateBasedOnCollider(vs.pathCollider, CursorType.PossibleRaid, CursorType.PendingRaid);

            if (Input.GetMouseButtonDown(0))
            {
                _raidEndPosition = Input.mousePosition;

                var worldPos = _raidEndPosition.Value.ToWorldPosition(Camera.main);

                if (vs.pathCollider.OverlapPoint(worldPos))
                {
                    StartCoroutine(StartRaid());

                    ExitRaidSetup();
                }
            }
        }
    }

    private void ExitRaidSetup()
    {
        _state = HorseRaidState.Idle;

        HorseRaidScreenIndicator.SetActive(false);

        _cursorManager.UseCursor(CursorType.Default);
    }

    private IEnumerator StartRaid()
    {
        RaidStarted?.Invoke();

        var raidEndPos = LevelUtils.GetNearestWaypoint(_raidEndPosition.Value.ToWorldPosition(Camera.main)) + (Vector3)UnityEngine.Random.insideUnitCircle * horseRaidPositionRandomFactor;

        raidEndIndicator.transform.position = _raidEndPosition.Value;
        raidEndIndicator.SetActive(true);

        Debug.Log("raid starting");
        _state = HorseRaidState.Idle;

        // var raidStartPos = _raidStartPosition.Value.ToWorldPosition(Camera.main);
        // var raidEndPos = _raidEndPosition.Value.ToWorldPosition(Camera.main);

        for (int i = 0; i < HorseRaiderCount; i++)
        {
            var horseRaiderObj = Instantiate(HorseRaiderPrefab, HorseRaiderSpawnPosition.position, Quaternion.identity);
            var horseRaiderComponent = horseRaiderObj.GetComponent<HorseRaiderV2>();

            _raidEndPosition = null;

            horseRaiderComponent.StartRaid(raidEndPos);
            horseRaiderComponent.PatrolStarted += OnPatrolStarted;

            yield return new WaitForSeconds(HorseRaiderSpawnDelay);
        }
    }

    private void OnPatrolStarted()
    {
        raidEndIndicator.SetActive(false);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        
        ExitRaidSetup();
    }
}

public interface ICleanable
{
    void CleanUp();
}