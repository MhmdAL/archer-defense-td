using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private HorseRaidState _state;

    private Vector2? _raidStartPosition;
    private Vector2? _raidEndPosition;

    private GameObject _raidStartIndicator = null;
    public GameObject raidEndIndicator;

    public AudioSource audioSource;
    public AudioClip raidStartSFX;

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

    public override void Execute()
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
                    OnAbilityActivated();

                    audioSource.PlayOneShot(raidStartSFX, GlobalManager.GlobalVolumeScale);

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

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartRaid()
    {
        RaidStarted?.Invoke();

        var raidEndWorldPos = _raidEndPosition.Value.ToWorldPosition(Camera.main);

        raidEndIndicator.transform.position = raidEndWorldPos;
        raidEndIndicator.SetActive(true);

        Debug.Log("raid starting");
        _state = HorseRaidState.Idle;
        
        _raidEndPosition = null;

        var paths = GenerateRaiderPaths(raidEndWorldPos).Shuffle().ToList();

        var currentPathIdx = 0;
        for (int i = 0; i < HorseRaiderCount; i++, currentPathIdx = (currentPathIdx + 1) % paths.Count)
        {
            var horseRaider = Instantiate(HorseRaiderPrefab, HorseRaiderSpawnPosition.position, Quaternion.identity).GetComponent<HorseRaiderV2>();

            var randomPath = paths[currentPathIdx];

            horseRaider.StartRaid(randomPath);
            horseRaider.PatrolStarted += OnPatrolStarted;

            yield return new WaitForSeconds(HorseRaiderSpawnDelay);
        }
    }

    private List<PathData> GenerateRaiderPaths(Vector2 targetPosition)
    {
        var paths = LevelUtils
            .GetNearestPaths(targetPosition, 7.5f)
            .Select(x => x.Item1.PathData.SubPath(x.Item2).ReversePath())
            .ToList();

        paths.ForEach(p => p.Waypoints.Add(targetPosition + UnityEngine.Random.insideUnitCircle * 4));

        return paths;
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

public enum HorseRaidState
{
    Idle,
    Setup,
}