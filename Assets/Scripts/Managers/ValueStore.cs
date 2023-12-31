using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityTimer;
using Unity.VisualScripting;

public enum ClickType
{
    Background,
    TowerBase,
    Tower
}

public enum GameStatus
{
    Win,
    Loss
}
// [ExecuteInEditMode]
public class ValueStore : MonoBehaviour
{
    public static ValueStore Instance;

    public event Action SilverChanged;
    public event Action LivesChanged;
    public event Action<GameStatus> LevelEnded;
    public event Action LevelStarted;

    public static float CurrentTime;

    public List<LevelTemplate> LevelPrefabs;
    public AudioProfile AudioProfile;

    public LevelTemplate CurrentLevel { get; set; }

    public GameObject buymenu, victoryMenu, defeatMenu, pauseMenu;
    public GameObject allEnemiesSlainEndMenuGoldDesc;

    public TowerManager towerManagerInstance;
    public MonsterManager monsterManagerInstance;
    public WaveManager waveManagerInstance;
    public AudioManager audioManagerInstance;
    public TimerManager timerManagerInstance;
    public AbilityManager abilityManagerInstance;
    public InfoPanelManager infoPanelManagerInstance;
    public InfoBoxManager infoBoxManagerInstance;
    public GameUIController uiControllerInstance;
    public UserClickHandler userClickHandlerInstance;
    public WaveSpawner WaveSpawner;
    public PotionSpawner PotionSpawner;

    public TextMeshProUGUI levelPopUpText;

    public TextMeshProUGUI endGameMenuGoldText;
    public TextMeshProUGUI endGameMenuSlainGoldText;
    public TextMeshProUGUI endGameMenuTotalGoldText;
    public TextMeshProUGUI endGameMenuWonText;
    public TextMeshProUGUI endGameMenuTitleText;

    public int TotalEnemiesSlain { get; private set; }

    private float silver;
    public float Silver
    {
        get
        {
            return silver;
        }
        set
        {
            silver = value;
            OnSilverChange();
        }
    }

    private int _lives;
    public int Lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;
            OnLivesChanged();
        }
    }

    private float easyGoldValue, intermediateGoldValue, expertGoldValue;
    public float CurrentGoldValue
    {
        get
        {
            return level.difficulty == LevelDifficulty.Easy ? easyGoldValue : level.difficulty == LevelDifficulty.Medium ? intermediateGoldValue : expertGoldValue;
        }
    }

    [HideInInspector] public float silverEarned;

    [HideInInspector] public Level level;

    [HideInInspector] public bool active;

    [HideInInspector] public Camera mainCamera;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioSource ambientAudioSource;
    [SerializeField]
    private AudioSource ambientWindAudioSource;

    public PolygonCollider2D pathCollider;

    void Awake()
    {
        active = true;
        Instance = this;
        _audioSource = GetComponent<AudioSource>();

        if (Application.isPlaying)
        {
            WaveSpawner.WaveEnded += OnWaveEnded;
            WaveSpawner.WaveStarted += OnWaveStarted;
            monsterManagerInstance.EnemyDied += OnEnemyDeath;
            // monsterManagerInstance.EnemyDied += WaveSpawner.OnEnemyDied;

            // SaveData s = DataService.Instance.SaveData;

            level = LevelsManager.CurrentLevel ?? new Level { levelID = 1 };

            // easyGoldValue = LevelsManager.easyGoldValues[level.levelID - 1];
            // intermediateGoldValue = LevelsManager.mediumGoldValues[level.levelID - 1];
            // expertGoldValue = LevelsManager.hardGoldValues[level.levelID - 1];
            // lives = (int)(SaveData.DEFAULT_LIVES + SaveData.GetUpgrade(UpgradeType.Lives).CurrentValue);
            // lives = 10;
            // Silver = startingSilver;

            levelPopUpText.text = level.levelName;

            mainCamera = Camera.main;

            ambientAudioSource.PlayOneShot(AudioProfile.ambient, GlobalManager.GlobalVolumeScale);
            ambientWindAudioSource.PlayOneShot(AudioProfile.ambient_wind, GlobalManager.GlobalVolumeScale);

            // UpdateStats();
        }
    }

    private void Start()
    {
        StartCoroutine(LoadLevel(level.levelID));
    }

    void Update()
    {
        CurrentTime = Time.time;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(LoadLevel((CurrentLevel.LevelId + 1) % (LevelPrefabs.Count + 1)));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(LoadLevel(CurrentLevel.LevelId - 1));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            WaveSpawner.SetWave(WaveSpawner.CurrentWave + 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            WaveSpawner.SetWave(WaveSpawner.CurrentWave - 1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GlobalManager.instance.LoadScene("menu", 1f);

            // SceneManager.LoadScene("menu");
        }
    }

    public IEnumerator LoadLevel(int levelId)
    {        
        if (CurrentLevel != null)
        {
            Destroy(CurrentLevel.gameObject);
        }

        yield return null;

        active = true;

        var levelPrefab = LevelPrefabs.First(x => x.LevelId == levelId);
        CurrentLevel = Instantiate(levelPrefab);

        pathCollider = GameObject.FindGameObjectWithTag("Path").GetComponent<PolygonCollider2D>();

        WaveSpawner.Reset(CurrentLevel.LevelData);

        if (CurrentLevel.SpawnPotions)
        {
            PotionSpawner.Reset(CurrentLevel.PotionSpawnBounds);
        }
        else
        {
            PotionSpawner.SetActive(false);
        }

        Silver = CurrentLevel.LevelData.StartingSilver;
        Lives = CurrentLevel.LevelData.StartingLives;

        TotalEnemiesSlain = 0;

        towerManagerInstance.Reset();
        towerManagerInstance.TowerBasesInScene.AddRange(CurrentLevel.TowerBases);

        monsterManagerInstance.Reset();
        monsterManagerInstance.paths = CurrentLevel.Paths.ToArray();

        uiControllerInstance.Reset();

        levelPopUpText.text = $"Level {levelId}";

        OnLevelStarted();
    }

    private void OnLevelStarted()
    {
        LevelStarted?.Invoke();

        _audioSource.PlayOneShot(AudioProfile.level_start, GlobalManager.GlobalVolumeScale);
    }

    public void OnSilverChange()
    {
        SilverChanged?.Invoke();
    }

    private void OnLivesChanged()
    {
        LivesChanged?.Invoke();
    }

    private void OnWaveStarted(int wave)
    {
        _audioSource.PlayOneShot(AudioProfile.wave_start, GlobalManager.GlobalVolumeScale);
    }

    private void OnWaveEnded(int wave)
    {
        if (!WaveSpawner.IsFinished)
        {
            _audioSource.PlayOneShot(AudioProfile.wave_end, GlobalManager.GlobalVolumeScale);
        }
        else if (active && Lives > 0)
        {
            GameOver(GameStatus.Win);
        }
    }

    public void OnEnemyDeath(Monster enemy, DamageSource source)
    {
        if (source == DamageSource.Normal)
        { // Killed by Archers
            TotalEnemiesSlain++;

            print("Enemy killed");

            float value = enemy.EnemyData.SilverValue * (1 + SaveData.GetUpgrade(UpgradeType.SilverIncrease)?.CurrentValue ?? 0);
            Silver += value;
            silverEarned += value;
        }
        else if (source == DamageSource.Exit)
        { // Killed by leaving screen
            Lives -= enemy.EnemyData.LivesValue;
        }

        if (Lives <= 0 && active)
        {
            Lives = 0;
            GameOver(GameStatus.Loss);
        }
    }

    public void LoadLevel(string levelToLoad)
    {
        GlobalManager.instance.LoadScene(levelToLoad, 1f);
    }

    public void RestartLevel()
    {
        GlobalManager.instance.LoadScene(SceneManager.GetActiveScene().name, 1f);
    }

    public void addGold()
    {
        //Silver += 1000;
    }

    public void GameOver(GameStatus gs)
    {
        active = false;

        var cleanables = FindObjectsOfType<MonoBehaviour>().OfType<ICleanable>();

        foreach (var cleanable in cleanables)
        {
            cleanable.CleanUp();
        }

        LevelEnded?.Invoke(gs);

        // Time.timeScale = 0f;

        SaveData save = DataService.Instance.SaveData;

        float winGoldGained = 0f;
        float allEnemiesSlainGoldGained = 0f;
        float totalGoldGained = 0f;

        if (gs == GameStatus.Win)
        {
            OnLevelVictory();

            // endGameMenuTitleText.text = "Victory!";
            // endGameMenuWonText.text = "Victory:-";
            // If level is won for the first time, set enemy slain count and gold gain accordingly
            if (!level.won)
            {
                level.totalEnemies = WaveSpawner.TotalEnemies;
                if (level.totalEnemiesSlain < TotalEnemiesSlain)
                {
                    level.totalEnemiesSlain = TotalEnemiesSlain;
                }

                winGoldGained = (float)(CurrentGoldValue * 0.8f) + ((CurrentGoldValue * 0.2f) * ((float)Lives / (10 + SaveData.GetUpgrade(UpgradeType.Lives)?.level ?? 0)));
                level.won = true;
            }
            // If level is already won, set enemy slain count and gold gain accordingly
            else
            {
                if (level.totalEnemiesSlain < TotalEnemiesSlain)
                {
                    level.totalEnemiesSlain = TotalEnemiesSlain;
                }

                winGoldGained = (CurrentGoldValue * 0.2f) * ((float)Lives / (10 + SaveData.GetUpgrade(UpgradeType.Lives)?.level ?? 0));
            }

            // If level is maxed for the first time, set gold gain accordingly
            if (!level.maxed && level.totalEnemiesSlain == level.totalEnemies)
            {
                allEnemiesSlainGoldGained = 0.4f * CurrentGoldValue;
                allEnemiesSlainEndMenuGoldDesc.SetActive(true);
                level.maxed = true;
            }

            LevelsManager.LevelWon(ref level);
        }
        else if (gs == GameStatus.Loss)
        {
            OnLevelDefeat();

            // endGameMenuTitleText.text = "Defeat";
            // endGameMenuWonText.text = "Defeat:-";

            // endGameMenuTitleText.color = Color.red;

            winGoldGained = (0.15f * CurrentGoldValue * ((float)WaveSpawner.CurrentWave / (float)WaveSpawner.TotalWaves));
        }

        totalGoldGained = winGoldGained + allEnemiesSlainGoldGained;

        endGameMenuGoldText.text = "+ " + Mathf.CeilToInt(winGoldGained);
        endGameMenuSlainGoldText.text = "+ " + Mathf.CeilToInt(allEnemiesSlainGoldGained);
        endGameMenuTotalGoldText.text = "+ " + Mathf.CeilToInt(totalGoldGained);

        save.Gold += Mathf.CeilToInt(totalGoldGained);

        DataService.Instance.WriteSaveData();

        // endGameMenu.SetActive(true);
    }

    private void OnLevelVictory()
    {
        _audioSource.PlayOneShot(AudioProfile.level_victory, GlobalManager.GlobalVolumeScale);

        this.AttachTimer(2f, (t) => victoryMenu.SetActive(true));
    }

    private void OnLevelDefeat()
    {
        _audioSource.PlayOneShot(AudioProfile.level_defeat, GlobalManager.GlobalVolumeScale);

        this.AttachTimer(1f, (t) => defeatMenu.SetActive(true));

        uiControllerInstance.SetScreenSaturation(-100);
    }

    public void OnGameOverProceedButtonClicked()
    {
        Time.timeScale = 1;

        // endGameMenu.SetActive(false);

        victoryMenu.SetActive(false);

        StartCoroutine(LoadLevel(CurrentLevel.LevelId + 1));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AnchorsToCorners(RectTransform t)
    {
        RectTransform pt = t.parent as RectTransform;

        if (t == null || pt == null) return;

        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
            t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
            t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);
    }
}
