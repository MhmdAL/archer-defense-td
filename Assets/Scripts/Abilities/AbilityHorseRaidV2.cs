using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityHorseRaidV2 : Ability<HorseRaidAbilityData>
{
    public Action RaidStarted;

    [Header("Raid Specific")]
    public GameObject HorseRaidScreenIndicator;

    public GameObject raidEndIndicator;

    private HorseRaidState _state;

    private Vector2? _raidEndPosition;

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

                    audioSource.PlayOneShot(AbilityData.raidStartSFX);

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
        for (int i = 0; i < AbilityData.HorseRaiderCount; i++, currentPathIdx = (currentPathIdx + 1) % paths.Count)
        {
            var horseRaider = Instantiate(AbilityData.HorseRaiderPrefab, AbilityData.HorseRaiderSpawnPosition.position, Quaternion.identity).GetComponent<HorseRaiderV2>();

            var randomPath = paths[currentPathIdx];

            horseRaider.StartRaid(randomPath);
            horseRaider.PatrolStarted += OnPatrolStarted;

            yield return new WaitForSeconds(AbilityData.HorseRaiderSpawnDelay);
        }
    }

    private List<PathData> GenerateRaiderPaths(Vector2 targetPosition)
    {
        const float acceptableDistance = 7.5f;

        var paths = LevelUtils
            .GetNearestPaths(targetPosition, acceptableDistance)
            .Select(x => x.path.PathData.SubPath(x.waypointIdx).ReversePath())
            .ToList();

        foreach (var path in paths)
        {
            var avgDirection = LevelUtils.CalculateAveragePathDirection(path, targetPosition);

            var leftRayDir = new Vector2(avgDirection.y, -avgDirection.x);
            var rightRayDir = new Vector2(-avgDirection.y, avgDirection.x);

            var leftHit = Physics2D.RaycastAll(path.Waypoints.Last(), leftRayDir).Where(x => x.collider.tag == "PathEdge").FirstOrDefault();
            var rightHit = Physics2D.RaycastAll(path.Waypoints.Last(), rightRayDir).Where(x => x.collider.tag == "PathEdge").FirstOrDefault();

            if (leftHit == default && rightHit == default)
            {
                Debug.LogWarning("Somehow, no path edge was found!");
                continue;
            }

            var nearestHit = leftHit.distance < rightHit.distance ? leftHit : rightHit;
            var nearestRay = leftHit.distance < rightHit.distance ? leftRayDir : rightRayDir;

            path.Waypoints.Add(nearestHit.point + nearestRay * UnityEngine.Random.Range(0.5f, 2f));
        }

        // find points on edges of path and add that to the path

        // paths.ForEach(p => p.Waypoints.Add(targetPosition + UnityEngine.Random.insideUnitCircle * 4));

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

        raidEndIndicator.SetActive(false);
    }
}

public enum HorseRaidState
{
    Idle,
    Setup,
}