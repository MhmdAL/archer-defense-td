using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityTimer;
using System;

public enum AbilityType{
	Arrow_Artillery,
	Damage_boost
}
public abstract class Ability : MonoBehaviour, IAttacker {

	public static event Action<AbilityType> AbilityActivated;

	public AbilityType t;
	public float baseCooldown;

	public Image cooldownImage;
	public Image activeIndicator, inactiveIndicator;

	protected ValueStore vs;

	protected Timer CooldownTimer;

	private Button _button;

	void Start () {
		vs = ValueStore.Instance;

		Initialize ();

		vs.WaveSpawner.WaveStarted += OnWaveStarted;
		vs.WaveSpawner.WaveEnded += OnWaveEnded;

		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnClick);

		SetReady (false);
	}

	public abstract void Initialize();
	public abstract void UpdateReadiness();
	public abstract void Activate();

	public void OnWaveStarted(int waveNumber){
		CooldownTimer.Resume ();
	}

	public void OnWaveEnded(int waveNumber){
		CooldownTimer.Pause ();
		SetReady (false);
	}

	void Update(){
		cooldownImage.fillAmount = CooldownTimer.GetTimeRemaining () / baseCooldown;

		UpdateReadiness ();
	}

	public void SetReady(bool active){
		if (active) {
			_button.interactable = true;
			activeIndicator.gameObject.SetActive (true);
			inactiveIndicator.gameObject.SetActive (false);
		} else {
			_button.interactable = false;
			activeIndicator.gameObject.SetActive (false);
			inactiveIndicator.gameObject.SetActive (true);
		}
	}

	public virtual void OnClick(){
		Activate ();

		if (AbilityActivated != null)
			AbilityActivated (t);

		SetReady (false);
		CooldownTimer.Restart (baseCooldown);
	}		
}
