using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public GameObject debugPanel;

    private ValueStore _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<ValueStore>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }

        if (debugPanel.activeSelf)
        {
            GameObject.Find("DBG_TIME_SCALE").GetComponent<TextMeshProUGUI>().text = $"TS: {Time.timeScale}";
            GameObject.Find("DBG_WAVE").GetComponent<TextMeshProUGUI>().text = $"Wave: {_gameManager.WaveSpawner.CurrentWave}";
            GameObject.Find("DBG_PLATOON").GetComponent<TextMeshProUGUI>().text = $"Platoon: {_gameManager.WaveSpawner.CurrentPlatoonIndex}";
            GameObject.Find("DBG_REMAINING_ENEMIES").GetComponent<TextMeshProUGUI>().text = $"ERTW: {_gameManager.WaveSpawner.EnemiesRemainingInCurrentWave}";
            GameObject.Find("DBG_WAVE_TIME").GetComponent<TextMeshProUGUI>().text = $"WT: {_gameManager.WaveSpawner.WaveTime}";
            GameObject.Find("DBG_TTNP").GetComponent<TextMeshProUGUI>().text = $"TTNP: {_gameManager.WaveSpawner.TimeTillNextPlatoon}";
            GameObject.Find("DBG_TOTAL_ENEMIES").GetComponent<TextMeshProUGUI>().text = $"TE: {_gameManager.WaveSpawner.TotalEnemies}";
            GameObject.Find("DBG_IS_SPAWNING").GetComponent<TextMeshProUGUI>().text = $"IS: {_gameManager.WaveSpawner.IsSpawning}";
        }
    }
}
