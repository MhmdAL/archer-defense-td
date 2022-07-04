using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum AbilityType{
	Arrow_Artillery,
	Damage_boost
}
public class Ability : MonoBehaviour, IAttacker {

	public delegate void AbilityActivatedEventHandler(AbilityType a);
	public static event AbilityActivatedEventHandler AbilityActivated;

	Button b;
	public AbilityType t;
	public float baseCooldown;
	[HideInInspector]	public float cooldown;

	public Image cooldownImage;
	public Image activeIndicator, inactiveIndicator;

	[HideInInspector]	public CooldownTimer cd;

	[HideInInspector]	public AbilityManager am;

	[HideInInspector]	public ValueStore vs;

	void Start () {
		vs = ValueStore.sharedInstance;

		InitializeValues ();

		vs.waveManagerInstance.WaveStarted += OnWaveStarted;
		vs.waveManagerInstance.WaveEnded += OnWaveEnded;
		vs.monsterManagerInstance.EnemyDied += OnEnemyDied;

		b = GetComponent<Button> ();
		b.onClick.AddListener (OnClick);

		SetReady (false);
	}

	public virtual void InitializeValues(){
		
	}

	public void OnEnemyDied(Monster m){
		
	}

	public void OnWaveStarted(int waveNumber){
		cd.Start ();
	}

	public virtual void UpdateReadiness(){
		
	}

	public void OnWaveEnded(int waveNumber){
		cd.Stop ();
		SetReady (false);
	}

	void Update(){
		cooldownImage.fillAmount = cd.GetCooldownRemaining () / baseCooldown;

		UpdateReadiness ();
	}

	public void SetReady(bool active){
		if (active) {
			b.interactable = true;
			activeIndicator.gameObject.SetActive (true);
			inactiveIndicator.gameObject.SetActive (false);
		} else {
			b.interactable = false;
			activeIndicator.gameObject.SetActive (false);
			inactiveIndicator.gameObject.SetActive (true);
		}
	}

	public virtual void OnClick(){
		Activate ();

		if (AbilityActivated != null)
			AbilityActivated (t);

		SetReady (false);
		cd.ResetTimer (baseCooldown);
	}

	public virtual void Activate(){
		// Activate the ability
	}
		
}
