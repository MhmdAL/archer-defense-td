using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityTimer;

/// <summary>
/// UI and Audio controller
/// </summary>
public class GameUIController : MonoBehaviour
{
    public event Action<object> ObjectClicked;

    public Tower LastFocusedTower
    {
        get
        {
            return _lastFocusedTower;
        }
        private set
        {
            if (_lastFocusedTower is not null)
            {
                _lastFocusedTower.SkillPointsChanged -= OnLastFocusedTowerSkillPointsChanged;
            }

            _lastFocusedTower = value;

            _lastFocusedTower.SkillPointsChanged += OnLastFocusedTowerSkillPointsChanged;

            UpdateTowerEnhancementsMenu(_lastFocusedTower);
        }
    }

    public Volume volume;

    private Tower _lastFocusedTower;

    public GameObject towerLock;
    public ParticleSystem waveStartParticles;

    [field: SerializeField]
    public TowerEnhancementMenu SpecialtyMenu { get; set; }
    [field: SerializeField]
    public GameObject TowerDesc { get; set; }

    /// <summary>
    /// UI Items
    /// </summary>

    [Header("UI items")]
    public TextMeshProUGUI TXT_livesText;
    public TextMeshProUGUI TXT_waveText;
    public TextMeshProUGUI TXT_silverText;

    public TextMeshProUGUI TXT_waveCountdown;

    public Button BTN_spawnWave;
    public GameObject GO_spawnWavePanel;
    public GameObject GO_spawnWavePanelParent;

    public GameObject GO_HUD;
    public GameObject GO_statsPanel;
    public GameObject GO_timeControlsPanel;

    public SkillDisplayUI ADSkillDisplay;
    public SkillDisplayUI ASSkillDisplay;
    public SkillDisplayUI ARSkillDisplay;

    public Image archerIcon;
    public Image upgradeCostImage;
    public Image upgradeIcon;

    public Image deployIconImage;
    public Image deployIconArcherImage;

    public Button upgradeButton;
    public Button deployIconButton;

    public TextMeshProUGUI archerTitleText;
    public TextMeshProUGUI archerLevelText;
    public TextMeshProUGUI nextUpgradeText;
    public TextMeshProUGUI nextUpgradeDescText;
    public TextMeshProUGUI upgradeCostText;

    public TextMeshProUGUI deployCostText;

    public GameObject deployIconBG;
    public GameObject deployIconArcher;

    public Color notEnoughSilverButtonColor, defaultButtonColor, defaultTextColor, notEnoughSilverTextColor;

    public Sprite upgradeSprite;
    public Sprite specialityUpgradeSprite;


    [field: SerializeField]
    public GameObject ArcherSpecialtyChoiceMenu { get; set; }

    [field: SerializeField]
    private BackgroundScaler background { get; set; }

    [SerializeField]
    private Image fadeInOut;
    [SerializeField]
    private float fadeInOutDuration;

    public GameObject horseRaidAbility;

    [SerializeField]
    private GameObject upcomingWaveIndicator;

    /// <summary>
    /// Sound Effects
    /// </summary>

    [Header("SFX")]
    [SerializeField]
    private AudioSource gameAudioSource;

    [SerializeField]
    private AudioClip platoonSpawnedSFX;

    /// <summary>
    /// Private stuff
    /// </summary>

    private TowerManager _towerManager;
    private ValueStore _vs;
    private ArcherDeployMenu _adm;

    public TutorialManager tutorialManager;

    public bool StartWaveEnabled { get; set; }

    public bool performLevelwiseInit = true;

    public GameObject blueVolume;

    private void Awake()
    {
        _towerManager = FindObjectOfType<TowerManager>();
        _vs = FindObjectOfType<ValueStore>();
        _adm = _vs.buymenu.GetComponent<ArcherDeployMenu>();

        _towerManager.TowerDeployed += OnTowerDeployed;
        _towerManager.TowerSold += OnTowerSold;
        _towerManager.TowerUpgraded += OnTowerUpgraded;
        _towerManager.TowerSpecialised += OnTowerSpecialised;
        _towerManager.TowerClicked += OnTowerClicked;
        _towerManager.TowerBaseClicked += OnTowerBaseClicked;

        _vs.SilverChanged += OnSilverChanged;
        _vs.LivesChanged += OnLivesChanged;

        _vs.WaveSpawner.WaveStarted += OnWaveStarted;
        _vs.WaveSpawner.WaveEnded += OnWaveEnded;
        _vs.WaveSpawner.WaveReset += OnWaveReset;
        _vs.WaveSpawner.PlatoonSpawned += OnPlatoonSpawned;
        _vs.userClickHandlerInstance.ObjectClicked += OnObjectClicked;

        _vs.LevelStarted += OnLevelStarted;

        _vs.WaveCountdownTimer.OnUpdate((f) =>
        {
            if (_vs.WaveSpawner.CurrentWave > 1)
            {
                TXT_waveCountdown.transform.parent.gameObject.SetActive(true);
                TXT_waveCountdown.text = $"00:{_vs.WaveCountdownDuration - f:00}";
            }
        });

        InitializeUI();
    }

    private void InitializeUI()
    {
        SetScreenSaturation(0);

        if (!performLevelwiseInit)
            return;

        if (_vs.level?.levelID == 1)
        {
            horseRaidAbility.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void OnLevelStarted()
    {
        FadeIn();
    }

    private void FadeIn()
    {
        fadeInOut.SetAlpha(1f);

        fadeInOut.DOFade(0f, fadeInOutDuration);
    }

    #region Tower Actions

    public void SellTower()
    {
        _vs.towerManagerInstance.SellTower(LastFocusedTower);
    }

    public void SetSpeciality(int archerId)
    {
        _vs.towerManagerInstance.SetSpeciality(LastFocusedTower, archerId);
    }

    public void UpgradeTower()
    {
        // if (LastFocusedTower.ArcherSpecialty != ArcherType.ClassicArcher)
        // {
        //     _towerManager.UpgradeTower(LastFocusedTower);
        // }
        // else
        // {
        UpdateTowerEnhancementsMenu(LastFocusedTower);

        SpecialtyMenu.gameObject.SetActive(true);
        // }
    }

    public void UseSkillpoint(int skill)
    {
        LastFocusedTower.UpgradeSkill((TowerSkillType)skill);

        UpdateTowerDesc(LastFocusedTower);
    }

    public void ApplyTowerEnhancement(int enhancementType)
    {
        _vs.towerManagerInstance.ApplyEnhancement(LastFocusedTower, (EnhancementType)enhancementType);

        SpecialtyMenu.gameObject.SetActive(false);
    }

    #endregion

    public void InitHUD()
    {
        BTN_spawnWave.interactable = true;
        GO_spawnWavePanel.GetComponent<Animator>().ResetTrigger("hide");
        GO_spawnWavePanel.GetComponent<Animator>().SetTrigger("show");
    }

    public void UpdateHUD()
    {
        UpdateAbilityAvailability();

        TXT_livesText.text = string.Concat(_vs.Lives);

        TXT_waveText.text = _vs.WaveSpawner.CurrentWave + "/" + _vs.WaveSpawner.TotalWaves;

        TXT_silverText.text = string.Concat(_vs.Silver);
    }

    public void EnableBlur()
    {
        blueVolume.SetActive(true);
    }

    public void DisableBlur()
    {
        blueVolume.SetActive(false);
    }

    private void UpdateAbilityAvailability()
    {
        // horseRaidAbility.SetActive(_vs.CurrentLevel.LevelId != 1 || !performLevelwiseInit || _vs.WaveSpawner.CurrentWave > 1);
    }

    public void SetScreenSaturation(float saturation)
    {
        if (volume.profile.TryGet<ColorAdjustments>(out var ca))
        {
            ca.saturation.value = saturation;
        }
    }

    public void UpdateDeployMenu()
    {
        if (_vs.Silver >= 50)
        {
            deployIconImage.color = defaultButtonColor;
            deployIconArcherImage.color = defaultButtonColor;
            deployIconButton.interactable = true;

            deployCostText.color = defaultTextColor;
        }
        else
        {
            deployIconImage.color = notEnoughSilverButtonColor;
            deployIconArcherImage.color = notEnoughSilverButtonColor;
            deployIconButton.interactable = false;

            deployCostText.color = notEnoughSilverTextColor;
        }
    }

    public void UpdateTowerDesc(Tower t)
    {
        if (t != null)
        {
            var specializationRequirements = _vs.towerManagerInstance.GetSpecializationRequirements(t.SpecializationLevel);

            // If next level is unlocked
            if (t.CurrentSkillLevel >= specializationRequirements.skillLevelRequired)
            {
                // Remove lock, activate upgradebutton
                towerLock.SetActive(false);
                upgradeButton.interactable = true;
                upgradeCostImage.gameObject.SetActive(true);
            }
            else
            { // if next level is locked
              // Activate lock, deactivate upgradebutton
                towerLock.SetActive(true);
                upgradeButton.interactable = false;
                upgradeCostImage.gameObject.SetActive(false);
            }
            // Update archer Icon, Title, Level to match currently clicked archer
            archerIcon.sprite = t.icon;
            archerTitleText.text = t.title;
            archerLevelText.text = "Level  " + string.Concat(t.level);

            // Disable upgrade button and next upgrade details if archer is maxlevel
            if (t.level == t.maxLevel)
            {
                upgradeIcon.gameObject.SetActive(false);
                nextUpgradeText.text = System.String.Empty;
                nextUpgradeDescText.text = System.String.Empty;
            }
            else
            {
                upgradeIcon.gameObject.SetActive(true);
            }

            if (t.level == 0)
            {
                upgradeIcon.sprite = specialityUpgradeSprite;
                nextUpgradeText.text = System.String.Empty;
                nextUpgradeDescText.text = System.String.Empty;
            }
            else if (t.level != t.maxLevel)
            {
                upgradeIcon.sprite = upgradeSprite;
                nextUpgradeDescText.text = GetNextUpgradeDescription(t);
                nextUpgradeText.text = "Next Upgrade";
            }

            upgradeCostText.text = string.Concat(specializationRequirements.silverCost);

            if (_vs.Silver >= specializationRequirements.silverCost && t.CurrentSkillLevel >= specializationRequirements.skillLevelRequired)
            {
                upgradeIcon.color = defaultButtonColor;
                upgradeButton.interactable = true;
                upgradeCostText.color = defaultTextColor;
                //if(t.level == 0)
                //upgradeCostText.GetComponent<TextMeshProUGUI> ().color = Color.yellow;
            }
            else
            {
                upgradeIcon.color = notEnoughSilverButtonColor;
                upgradeButton.interactable = false;

                upgradeCostText.color = notEnoughSilverTextColor;
            }

            ADSkillDisplay.CurrentLevel = t.ADSkill.CurrentLevel;
            ASSkillDisplay.CurrentLevel = t.ASSkill.CurrentLevel;
            ARSkillDisplay.CurrentLevel = t.ARSkill.CurrentLevel;

            if (t.SkillPoints > 0)
            {
                ArcherSpecialtyChoiceMenu.SetActive(true);
            }
            else
            {
                ArcherSpecialtyChoiceMenu.SetActive(false);
            }
        }
    }

    private string GetNextUpgradeDescription(Tower t)
    {
        string x = "";
        for (int i = 0; i < t.towerUpgrades.Count; i++)
        {
            if (t.towerUpgrades[i].intendedLevel == t.level + 1)
            {
                x += "<#54DFFBFF>+" + Mathf.Abs(t.towerUpgrades[i].value) * 100 + "%</color> " + GetDisplayName(t.towerUpgrades[i].type) + "\n";
            }
        }
        return x;
    }

    private string GetDisplayName(Type t)
    {
        switch (t)
        {
            case Type.ATTACK_DAMAGE:
                return "Attack Damage";
                break;
            case Type.ATTACK_SPEED:
                return "Attack Speed";
                break;
            case Type.ATTACK_RANGE:
                return "Attack Range";
                break;
            case Type.SLOW_ON_ATTACK:
                return "Slow";
                break;
            default:
                return "";
                break;
        }
    }

    public void UpdateTowerEnhancementsMenu(Tower t)
    {
        var nextPossibleEnhancements = _vs.towerManagerInstance.GetNextPossibleEnhancements(t);

        SpecialtyMenu.SetEnhancements(nextPossibleEnhancements);
    }


    private void OnObjectClicked(object obj)
    {
        if (obj is BackgroundScaler bg)
        {
            OnBackgroundClicked();
        }
        else if (obj is Tower t)
        {
            OnTowerClicked(t);
        }
        else if (obj is TowerBase tb)
        {
            OnTowerBaseClicked(tb);
        }
    }

    private void OnTowerDeployed(Tower t)
    {
        Debug.Log("Tower deployed");

        LastFocusedTower = t;

        _vs.buymenu.SetActive(false);

        UpdateTowerDesc(t);

        TowerDesc.SetActive(true);
    }

    private void OnTowerSold()
    {
        SpecialtyMenu.gameObject.SetActive(false);
        TowerDesc.SetActive(false);
    }

    private void OnTowerUpgraded(Tower t)
    {
        SpecialtyMenu.gameObject.SetActive(false);
        TowerDesc.SetActive(false);
    }

    private void OnTowerSpecialised(Tower t)
    {
        Debug.Log("Tower specialized");

        LastFocusedTower = t;

        SpecialtyMenu.gameObject.SetActive(false);

        UpdateTowerDesc(t);
    }

    private void OnTowerClicked(Tower t)
    {
        LastFocusedTower = t;

        _vs.buymenu.SetActive(false);

        UpdateTowerDesc(t);

        SpecialtyMenu.gameObject.SetActive(false);
        TowerDesc.SetActive(true);
    }

    private void OnTowerBaseClicked(TowerBase tb)
    {
        _adm.transform.position = new Vector3(10000, 10000, 0);
        _adm.objToFollow = tb.gameObject;
        _adm.tb = tb;

        _vs.buymenu.SetActive(true);
        _vs.buymenu.transform.SetAsLastSibling();
        _vs.pauseMenu.transform.SetAsLastSibling();

        SpecialtyMenu.gameObject.SetActive(false);
        TowerDesc.SetActive(false);

        UpdateDeployMenu();
    }

    private void OnBackgroundClicked()
    {
        _vs.buymenu.SetActive(false);
        SpecialtyMenu.gameObject.SetActive(false);
        TowerDesc.SetActive(false);
    }

    private void OnSilverChanged()
    {
        if (TowerDesc.activeSelf)
        {
            UpdateTowerDesc(LastFocusedTower);
        }

        if (_vs.buymenu.activeSelf)
        {
            UpdateDeployMenu();
        }

        UpdateHUD();
    }

    private void OnLivesChanged()
    {
        UpdateHUD();
    }

    private void OnLastFocusedTowerSkillPointsChanged(Tower tower)
    {
        UpdateTowerDesc(tower);
    }

    public void OnStartWaveButtonClicked()
    {
        Instantiate(waveStartParticles, BTN_spawnWave.transform.position.ToWorldPosition(Camera.main), waveStartParticles.transform.rotation);

        BTN_spawnWave.interactable = false;

        TXT_waveCountdown.transform.parent.gameObject.SetActive(false);

        _vs.WaveSpawner.SpawnNextWave();
    }

    private void OnWaveStarted(int wave)
    {
        var animator = GO_spawnWavePanel.GetComponent<Animator>();

        this.AttachTimer(0.5f, (t) =>
        {
            animator.ResetTrigger("show");
            animator.SetTrigger("hide");
        });

        UpdateHUD();
    }

    private void OnWaveEnded(int wave)
    {
        Debug.Log("Wave Ended");

        if (!_vs.WaveSpawner.LevelFinished)
        {
            BTN_spawnWave.interactable = true;
            GO_spawnWavePanel.GetComponent<Animator>().SetTrigger("show");
        }
    }

    private void OnWaveReset(int wave)
    {
        if (!_vs.WaveSpawner.LevelFinished)
        {
            BTN_spawnWave.interactable = true;
            GO_spawnWavePanel.GetComponent<Animator>().SetTrigger("show");
        }
    }

    public void ShowWaveMenu(bool isEnabled = true)
    {
        BTN_spawnWave.interactable = isEnabled;
        GO_spawnWavePanel.GetComponent<Animator>().SetTrigger("show");
    }

    public void HideWaveMenu()
    {
        GO_spawnWavePanel.GetComponent<Animator>().SetTrigger("hide");
    }

    private void OnPlatoonSpawned(Platoon platoon)
    {
        Debug.Log("Platoon Spawned");

        gameAudioSource.PlayOneShot(platoonSpawnedSFX, GlobalManager.GlobalVolumeScale);
    }

    public void Reset()
    {
        Debug.Log("Game Resetting");

        InitHUD();
    }

    private void OnDestroy()
    {
        _vs.WaveSpawner.WaveStarted -= OnWaveStarted;
        _vs.WaveSpawner.WaveEnded -= OnWaveEnded;

        _towerManager.TowerDeployed -= OnTowerDeployed;
        _towerManager.TowerSold -= OnTowerSold;
        _towerManager.TowerUpgraded -= OnTowerUpgraded;
        _towerManager.TowerSpecialised -= OnTowerSpecialised;
        _towerManager.TowerClicked -= OnTowerClicked;
        _towerManager.TowerBaseClicked -= OnTowerBaseClicked;

        _vs.SilverChanged -= OnSilverChanged;
        _vs.LivesChanged -= OnLivesChanged;

        _vs.userClickHandlerInstance.ObjectClicked -= OnObjectClicked;

        _vs.LevelStarted -= OnLevelStarted;
    }
}