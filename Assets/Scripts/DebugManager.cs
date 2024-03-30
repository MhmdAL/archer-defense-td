using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public GameObject debugPanel;

    private ValueStore _gameManager;

    public GameObject TXT_selected;

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
            GameObject.Find("DBG_WST").GetComponent<TextMeshProUGUI>().text = $"WST: {_gameManager.WaveStartTime}";

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            if (hits.Any())
            {
                var hit = hits[0];

                var text = new StringBuilder();
                text.AppendLine("Entity: " + hit.collider.name);

                var monster = hit.collider.gameObject.GetComponentInChildren<Monster>();
                if (monster != null)
                {
                    text.AppendLine("Speed: " + monster.movementTracker.CurrentVelocity);
                }

                var horseArcher = hit.collider.gameObject.GetComponentInChildren<HorseRaiderV2>();
                if (horseArcher != null)
                {
                    text.AppendLine("Speed: " + horseArcher.movementTracker.CurrentVelocity);
                }

                TXT_selected.GetComponent<TextMeshProUGUI>().text = text.ToString();
            }
            else
            {
                TXT_selected.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
}
