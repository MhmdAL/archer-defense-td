using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityTimer;
using System;
using System.Collections.Generic;

public enum AbilityType
{
    Arrow_Artillery,
    Damage_boost,
    Cavalry_Raid,
}
public abstract class Ability : MonoBehaviour, IAttacking, ICleanable
{
    public static event Action<AbilityType> AbilityActivated;

    public AbilityType t;
    public float baseCooldown;

    public Image cooldownImage;
    public Image activeIndicator, inactiveIndicator;

    protected ValueStore vs;

    protected Timer CooldownTimer;

    private Button _button;

    public List<TargetHitEffect> OnHitEffects => throw new NotImplementedException();

    protected virtual void Awake()
    {

    }

    void Start()
    {
        vs = ValueStore.Instance;

        Initialize();

        vs.WaveSpawner.WaveStarted += OnWaveStarted;
        vs.WaveSpawner.WaveEnded += OnWaveEnded;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);

        SetReady(false);
    }

    public virtual void Initialize()
    {
        CooldownTimer = this.AttachTimer(0, null, isDoneWhenElapsed: false);
    }

    protected abstract bool IsReady();
    public abstract void Activate();

    public virtual void UpdateReadiness(params Func<bool>[] readyConditions)
    {
        var isReady = IsReady();

        SetReady(isReady);
    }

    protected virtual bool CooldownFinished() => CooldownTimer.GetTimeRemaining() <= 0;

    public void OnWaveStarted(int waveNumber)
    {
        CooldownTimer.Resume();
    }

    public void OnWaveEnded(int waveNumber)
    {
        CooldownTimer.Pause();
        SetReady(false);
    }

    protected virtual void Update()
    {
        cooldownImage.fillAmount = CooldownTimer.GetTimeRemaining() / baseCooldown;

        UpdateReadiness();
    }

    public void SetReady(bool active)
    {
        if (active)
        {
            _button.interactable = true;
            activeIndicator.gameObject.SetActive(true);
            inactiveIndicator.gameObject.SetActive(false);
        }
        else
        {
            _button.interactable = false;
            activeIndicator.gameObject.SetActive(false);
            inactiveIndicator.gameObject.SetActive(true);
        }
    }

    public virtual void OnClick()
    {
        Activate();

        AbilityActivated?.Invoke(t);

        SetReady(false);
        CooldownTimer.Restart(baseCooldown);
    }

    public virtual void CleanUp()
    {
        CooldownTimer.Restart(0);
    }
}
