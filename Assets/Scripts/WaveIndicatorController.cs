using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes2D;
using TMPro;
using UnityEngine;

public class WaveIndicatorController : MonoBehaviour
{
    [SerializeField]
    private float IndicatorThreshhold = 5f;
    
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip upcomingPlatoonSFX;

    private ValueStore _gameManager;
    private List<UpcomingPlatoonIndicator> _upcomingPlatoonIndicators = new List<UpcomingPlatoonIndicator>();

    private bool _doneForCurrentPlatoon;

    private void Awake()
    {
        _gameManager = FindObjectOfType<ValueStore>();

        _gameManager.LevelStarted += OnLevelStarted;
        _gameManager.WaveSpawner.PlatoonSpawned += OnPlatoonSpawned;
    }

    private void Update()
    {
        UpdateUpcomingIndicators();
    }

    private void OnLevelStarted()
    {
        _upcomingPlatoonIndicators.Clear();
        _upcomingPlatoonIndicators = _gameManager.CurrentLevel.gameObject.GetComponentsInChildren<UpcomingPlatoonIndicator>(true).ToList();
    }

    private void OnPlatoonSpawned(Platoon platoon)
    {
        _doneForCurrentPlatoon = false;

        foreach(var indicator in _upcomingPlatoonIndicators)
        {
            indicator.gameObject.SetActive(false);
        }
    }

    private void UpdateUpcomingIndicators()
    {
        if (!_gameManager.WaveSpawner.IsSpawning)
            return;

        if (!_doneForCurrentPlatoon && _gameManager.WaveSpawner.TimeTillNextPlatoon <= IndicatorThreshhold && _gameManager.WaveSpawner.NextPlatoon != null)
        {
            _doneForCurrentPlatoon = true;

            audioSource.PlayOneShot(upcomingPlatoonSFX, GlobalManager.GlobalVolumeScale);

            var entranceIds = _gameManager.WaveSpawner.NextPlatoon.Squads.Select(x => x.EntranceId);

            foreach (var entId in entranceIds)
            {
                if (_upcomingPlatoonIndicators.FirstOrDefault(x => x.entranceId == entId) != null)
                {
                    _upcomingPlatoonIndicators.First(x => x.entranceId == entId).gameObject.SetActive(true);
                }
            }
        }
    }

}
