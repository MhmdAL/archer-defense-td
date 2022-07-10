using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public GameObject towerLock;

    [field: SerializeField]
    public GameObject SpecialtyMenu { get; set; }
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


    private TowerManager _towerManager;
    private ValueStore _vs;

    private void Awake()
    {
        _towerManager = FindObjectOfType<TowerManager>();
        _vs = ValueStore.sharedInstance;

        _towerManager.TowerDeployed += OnTowerDeployed;
        _towerManager.TowerSold += OnTowerSold;
        _towerManager.TowerUpgraded += OnTowerUpgraded;
        _towerManager.TowerSpecialised += OnTowerSpecialised;

        _vs.SilverChanged += OnSilverChanged;
    }

    private void OnTowerDeployed(Tower t)
    {
        _vs.buymenu.SetActive(false);

        UpdateTowerDesc(t);
    }

    private void OnTowerSold()
    {
        SpecialtyMenu.SetActive(false);
        TowerDesc.SetActive(false);
    }

    private void OnTowerUpgraded(Tower t)
    {
        SpecialtyMenu.SetActive(false);
        TowerDesc.SetActive(false);
    }

    private void OnTowerSpecialised(Tower t)
    {
        SpecialtyMenu.SetActive(false);
        
        UpdateTowerDesc(t);
    }

    private void OnSilverChanged()
    {
        if (_vs.lastClicked != null)
        {
            if (_vs.lastClickType == ClickType.Tower)
            {
                UpdateTowerDesc(_vs.lastClickedTower);
            }
            else if (_vs.lastClickType == ClickType.TowerBase)
            {
                UpdateDeployMenu();
            }
        }
    }

    public void UpgradeTower()
    {
        var t = _vs.lastClickedTower;
        if (t.archerSpeciality != ArcherType.ClassicArcher)
        {
            _towerManager.UpgradeTower(t);
        }
        else
        {
            SpecialtyMenu.SetActive(true);
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
            // If next level is unlocked
            if (Tower.CanUpgrade(t))
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

            upgradeCostText.text = string.Concat(t.UpgradeCost);

            if (_vs.Silver >= t.UpgradeCost)
            {
                upgradeIcon.color = defaultButtonColor;
                upgradeButton.interactable = true;
                upgradeCostText.color = defaultTextColor;
                //if(t.level == 0)
                //upgradeCostText.GetComponent<TextMeshProUGUI> ().color = Color.yellow;
            }
            else if (_vs.Silver < t.UpgradeCost)
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

    private void OnDestroy()
    {
        _towerManager.TowerDeployed -= OnTowerDeployed;
        _towerManager.TowerSold -= OnTowerSold;
        _towerManager.TowerUpgraded -= OnTowerUpgraded;
        _towerManager.TowerSpecialised -= OnTowerSpecialised;

        _vs.SilverChanged -= OnSilverChanged;
    }
}