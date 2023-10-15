using UnityEngine;

public class AbilityHorseRaidV2 : Ability
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


    public override void Activate()
    {
        _state = HorseRaidState.SetupPhase;
        HorseRaidScreenIndicator.SetActive(true);

        Debug.Log("Activating horse raid");
    }

    protected override void Update()
    {
        base.Update();

        if (_state == HorseRaidState.SetupPhase && Input.GetMouseButtonDown(0))
        {
            _raidEndPosition = Input.mousePosition;

            // _raidEndIndicator = Instantiate(HorseRaidEndIndicatorPrefab, _raidEndPosition.Value.ToWorldPosition(Camera.main), Quaternion.identity);

            StartRaid();
        }
    }

    private void StartRaid()
    {
        HorseRaidScreenIndicator.SetActive(false);

        Debug.Log("raid starting");
        _state = HorseRaidState.Commencing;

        // var raidStartPos = _raidStartPosition.Value.ToWorldPosition(Camera.main);
        var raidEndPos = _raidEndPosition.Value.ToWorldPosition(Camera.main);

        for (int i = 0; i < HorseRaiderCount; i++)
        {
            var horseRaiderObj = Instantiate(HorseRaiderPrefab, HorseRaiderSpawnPosition.position, Quaternion.identity);
            var horseRaiderComponent = horseRaiderObj.GetComponent<HorseRaiderV2>();

            _raidEndPosition = null;

            horseRaiderComponent.StartPatrol(raidEndPos);
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
