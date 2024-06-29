using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityTimer;
using UnityEngine.AI;
using DG.Tweening;

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

    public TimeState timeState;

    public event Action SilverChanged;
    public event Action LivesChanged;
    public event Action<GameStatus> LevelEnded;
    public event Action LevelStarted;

    public static float CurrentTime;

    public List<LevelTemplate> LevelPrefabs;
    public AudioProfile AudioProfile;

    public LevelTemplate CurrentLevel { get; set; }

    public GameObject buymenu, victoryMenu, defeatMenu, pauseMenu;
    public TextMeshProUGUI victoryText;
    public GameObject victoryPanel;
    public TextMeshProUGUI defeatText;
    public GameObject defeatPanel;
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
    public SettingsPanel SettingsPanel;

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

    public float WaveStartTime => WaveCountdownTimer.GetTimeRemaining();

    public Timer WaveCountdownTimer;
    public float WaveCountdownDuration;

    private void Awake()
    {
        active = false;
        Instance = this;
        _audioSource = GetComponent<AudioSource>();

        WaveCountdownTimer = this.AttachTimer(WaveCountdownDuration, (t) =>
        {
            WaveSpawner.SpawnNextWave();
            t.Restart(WaveCountdownDuration);
            t.Pause();

        }, isDoneWhenElapsed: false);

        WaveCountdownTimer.Pause();

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

            // UpdateStats();
        }
    }

    private void Start()
    {
        var levelToStart = 1;

        if (GlobalManager.GlobalState.TryGetValue("InitialLevel", out var levelId))
        {
            levelToStart = (int)levelId;
        }

        StartCoroutine(LoadLevelAsync(levelToStart));

        AudioUtils.FadeInAllSounds(false, 5f);
    }

    private void Update()
    {
        CurrentTime = Time.time;

        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(DefeatSequence());
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(LoadLevelAsync((CurrentLevel.LevelId + 1) % (LevelPrefabs.Count + 1)));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(LoadLevelAsync(CurrentLevel.LevelId - 1));
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

    public void LoadLevel(int levelId)
    {
        StartCoroutine(LoadLevelAsync(levelId));
    }

    public void RestartLevel()
    {
        // GlobalManager.instance.LoadScene("Test");
        StartCoroutine(LoadLevelAsync(CurrentLevel.LevelId));
    }

    private IEnumerator LoadLevelAsync(int levelId)
    {
        if (!LevelPrefabs.Select(x => x.LevelId).Contains(levelId))
            yield break;

        yield return PreGameSetup();

        uiControllerInstance.SetHudInteractable(false);

        var levelPrefab = LevelPrefabs.First(x => x.LevelId == levelId);
        CurrentLevel = Instantiate(levelPrefab);

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

        towerManagerInstance.TowerBasesInScene.AddRange(CurrentLevel.TowerBases);
        monsterManagerInstance.paths = CurrentLevel.Paths.ToArray();

        WaveSpawner.SetLevel(CurrentLevel.LevelData);

        levelPopUpText.text = $"Level {GetLevelDisplay(levelId)}";

        ambientAudioSource.PlayOneShot(SoundEffects.AMBIENT_1);
        // ambientWindAudioSource.PlayOneShot(SoundEffects.AMBIENT_2);

        active = true;

        OnLevelStarted();
    }

    private void OnLevelStarted()
    {
        LevelStarted?.Invoke();

        _audioSource.PlayOneShot(SoundEffects.LEVEL_START);

        this.AttachTimer(2f, (f) => uiControllerInstance.SetHudInteractable(true));
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
        _audioSource.PlayOneShot(SoundEffects.WAVE_START);

        WaveCountdownTimer.Restart(WaveCountdownDuration);
        WaveCountdownTimer.Pause();
    }

    private void OnWaveEnded(int wave)
    {
        if (!WaveSpawner.LevelFinished)
        {
            _audioSource.PlayOneShot(SoundEffects.WAVE_END);

            WaveCountdownTimer.Restart(WaveCountdownDuration);
            WaveCountdownTimer.Resume();
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

    /// <summary>
    /// Restores the game state to its initial state. As though the scene was restarted.
    /// </summary>
    private IEnumerator PreGameSetup()
    {
        if (CurrentLevel != null)
        {
            Destroy(CurrentLevel.gameObject);
        }

        yield return null;

        _audioSource.Stop();
        ambientAudioSource.Stop();
        ambientWindAudioSource.Stop();
        ambientAudioSource.volume = 1;
        ambientWindAudioSource.volume = 1;

        SettingsPanel.SetTimeScale(TimeScaleState.Normal);

        CleanUpCleanables();

        TotalEnemiesSlain = 0;

        towerManagerInstance.Reset();

        monsterManagerInstance.Reset();

        WaveSpawner.Reset();

        pauseMenu.SetActive(false);
        victoryMenu.SetActive(false);
        defeatMenu.SetActive(false);

        uiControllerInstance.Reset();
    }

    private void PostGameCleanUp()
    {
        ambientAudioSource.DOFade(0f, 1f).onComplete += () => ambientAudioSource.Stop();
        ambientWindAudioSource.DOFade(0f, 1f).onComplete += () => ambientWindAudioSource.Stop();

        SettingsPanel.SetTimeScale(TimeScaleState.Normal);

        uiControllerInstance.SetHudInteractable(false);

        CleanUpCleanables();
    }

    public void CleanUpCleanables()
    {
        var cleanables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICleanable>();

        foreach (var cleanable in cleanables)
        {
            cleanable.CleanUp();
        }
    }

    public void GameOver(GameStatus gs)
    {
        active = false;

        LevelEnded?.Invoke(gs);

        PostGameCleanUp();

        if (gs == GameStatus.Win)
        {
            OnLevelVictory();
        }
        else if (gs == GameStatus.Loss)
        {
            OnLevelDefeat();
        }
    }

    private void OnLevelVictory()
    {
        StartCoroutine(VictorySequence());

        PlayerPrefs.SetInt("LastCompletedLevel", CurrentLevel.LevelId);
    }

    private void OnLevelDefeat()
    {
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator VictorySequence()
    {
        yield return new WaitForSeconds(2.5f);

        _audioSource.PlayOneShot(SoundEffects.LEVEL_END_VICTORY);

        uiControllerInstance.GO_HUD.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        victoryMenu.GetComponent<CanvasGroup>().alpha = 0f;
        victoryText.GetComponent<TextMeshProUGUI>().alpha = 0f;
        victoryPanel.GetComponent<CanvasGroup>().alpha = 0f;

        victoryMenu.SetActive(true);
        victoryText.gameObject.SetActive(true);
        var tween = victoryMenu.GetComponent<CanvasGroup>().DOFade(1f, 1f);

        tween.onComplete = () =>
        {
            var textTween = victoryText.DOFade(1f, 2.5f);
            textTween.onComplete = () =>
            {
                this.AttachTimer(0.5f, (f) =>
                {
                    // uiControllerInstance.EnableBlur();
                    victoryPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.75f);
                });
            };
        };
    }

    private IEnumerator DefeatSequence()
    {
        yield return new WaitForSeconds(2.5f);

        _audioSource.PlayOneShot(SoundEffects.LEVEL_END_DEFEAT);

        uiControllerInstance.GO_HUD.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        defeatMenu.GetComponent<CanvasGroup>().alpha = 0f;
        defeatText.GetComponent<TextMeshProUGUI>().alpha = 0f;
        defeatPanel.GetComponent<CanvasGroup>().alpha = 0f;

        defeatMenu.SetActive(true);
        defeatText.gameObject.SetActive(true);
        var tween = defeatMenu.GetComponent<CanvasGroup>().DOFade(1f, 1f);

        tween.onComplete = () =>
        {
            uiControllerInstance.SetScreenSaturation(SaturationState.Unsaturated);

            var textTween = defeatText.DOFade(1f, 2.5f);
            textTween.onComplete = () =>
            {
                this.AttachTimer(0.5f, (f) =>
                {
                    defeatPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.75f);
                });
            };
        };
    }

    public void OnGameOverProceedButtonClicked()
    {
        Time.timeScale = 1;

        // endGameMenu.SetActive(false);

        victoryMenu.SetActive(false);

        StartCoroutine(LoadLevelAsync(CurrentLevel.LevelId + 1));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private string GetLevelDisplay(int level) => level switch
    {
        1 => "One",
        2 => "Two",
        3 => "Three",
        4 => "Four",
        5 => "Five",
        _ => "One"
    };

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

public enum LevelPhaseState
{
    Setup,
    Defending
}

public enum TimeState
{
    Playing,
    Paused
}

public enum ResultState
{
    Victory,
    Defeat
}