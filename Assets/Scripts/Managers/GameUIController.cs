using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private Tower _lastFocusedTower;

    public GameObject towerLock;

    [field: SerializeField]
    public TowerEnhancementMenu SpecialtyMenu { get; set; }
    [field: SerializeField]
    public GameObject TowerDesc { get; set; }

    [Header("UI items")]
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

    private TowerManager _towerManager;
    private ValueStore _vs;
    private ArcherDeployMenu _adm;

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

        // background.BackgroundClicked += OnBackgroundClicked;
        _vs.userClickHandlerInstance.ObjectClicked += OnObjectClicked;
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
    }

    private void OnLastFocusedTowerSkillPointsChanged(Tower tower)
    {
        UpdateTowerDesc(tower);
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
        LastFocusedTower.UpgradeSkill((TowerSkill)skill);
    }

    public void ApplyTowerEnhancement(int enhancementType)
    {
        _vs.towerManagerInstance.ApplyEnhancement(LastFocusedTower, (EnhancementType)enhancementType);

        SpecialtyMenu.gameObject.SetActive(false);
    }

    #endregion

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
                nextUpgradeDescText.text = t.NextUpgradeStats();
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

    public void UpdateTowerEnhancementsMenu(Tower t)
    {
        var nextPossibleEnhancements = _vs.towerManagerInstance.GetNextPossibleEnhancements(t);

        SpecialtyMenu.SetEnhancements(nextPossibleEnhancements);
    }

    private void OnDestroy()
    {
        _towerManager.TowerDeployed -= OnTowerDeployed;
        _towerManager.TowerSold -= OnTowerSold;
        _towerManager.TowerUpgraded -= OnTowerUpgraded;
        _towerManager.TowerSpecialised -= OnTowerSpecialised;
        _towerManager.TowerClicked -= OnTowerClicked;
        _towerManager.TowerBaseClicked -= OnTowerBaseClicked;

        _vs.SilverChanged -= OnSilverChanged;

        _vs.userClickHandlerInstance.ObjectClicked -= OnObjectClicked;
    }
}