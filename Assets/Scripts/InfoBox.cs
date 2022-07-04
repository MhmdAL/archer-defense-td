using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

[Serializable]
public class BoolEvaluator{

	[PopUpAttribute(new string[]{"FirstWaveStarted", "FirstArcherDeployed", "FirstArcherSpecialised", "ArtilleryUsed",
		"DamageBoostUsed", "FirstSuperArcherDeployed"})]
	public string methodName;

	public bool value; 
}

[Serializable]
public class SerializableAction{
	[PopUpAttribute(new string[]{"OnTowerInfoDeactivated", "OnTowerInfoActivated", "None"})]
	public string methodName = "None";
}

public class InfoBox : MonoBehaviour {

	public List<BoolEvaluator> appearConditions;
	public List<BoolEvaluator> disappearConditions;

	public SerializableAction onAppear;
	public SerializableAction onDisappear;

	private delegate bool ConditionMethod();
	private List<ConditionMethod> appearMethods = new List<ConditionMethod> ();
	private List<ConditionMethod> disappearMethods = new List<ConditionMethod> ();

	private delegate void OnCommand ();
	private OnCommand onAppearMethod;
	private OnCommand onDisappearMethod;

	private RectTransform myTransform;

	[HideInInspector]	public InfoBoxManager ibm;

	void Awake(){
		ibm = ValueStore.sharedInstance.infoBoxManagerInstance;	
	}

	void Start(){
		myTransform = transform.parent as RectTransform;
	}

	public void OnEnable(){
		if(ibm != null)
			ibm.UpdateBackground();

		if(onAppearMethod != null)
			onAppearMethod ();
	}

	public void UpdateState(){
		Level x = LevelsManager.GetLevel (ValueStore.sharedInstance.level.levelID);
		if (x.won) {
			Destroy (transform.parent.gameObject);
		}
	}

	public void SetDelegates(){
		foreach (var item in appearConditions) {
			appearMethods.Add (Delegate.CreateDelegate (typeof(ConditionMethod), ibm, item.methodName) as ConditionMethod);
		}

		foreach (var item in disappearConditions) {
			disappearMethods.Add (Delegate.CreateDelegate (typeof(ConditionMethod), ibm, item.methodName) as ConditionMethod);
		}

		if(onAppear.methodName != "None")
			onAppearMethod += Delegate.CreateDelegate (typeof(OnCommand), ibm, onAppear.methodName) as OnCommand;

		if(onDisappear.methodName != "None")
			onDisappearMethod += Delegate.CreateDelegate (typeof(OnCommand), ibm, onDisappear.methodName) as OnCommand;
	}

	public void UpdateVisibility(){
		if(appearConditions.Count > 0)
			CheckAppearConditions ();

		if(disappearConditions.Count > 0)
			CheckDisappearConditions ();
	}

	public void CheckAppearConditions(){
		for (int i = 0; i < appearMethods.Count; i++) {
			if (appearMethods[i] != null && appearMethods [i].Invoke () != appearConditions [i].value) {
				return;
			}
		}
		ShowBox ();
		if(onAppearMethod != null)
			onAppearMethod ();
	}

	public void CheckDisappearConditions(){
		for (int i = 0; i < disappearMethods.Count; i++) {
			if (disappearMethods[i] != null && disappearMethods [i].Invoke () != disappearConditions [i].value) {
				return;
			}
		}
		HideBox ();
		if(onDisappearMethod != null)
			onDisappearMethod ();
	}

	public void ShowBox(){
		transform.parent.gameObject.SetActive (true);
	}

	public void HideBox(){
		transform.parent.gameObject.SetActive (false);

		if(ibm != null)
			ibm.UpdateBackground();

		if(onDisappearMethod != null)
			onDisappearMethod ();
	}

	void OnDisable(){
		if(ibm != null)
			ibm.UpdateBackground();

		if(onDisappearMethod != null)
			onDisappearMethod ();
	}

	void OnDestroy(){
		if (ibm != null) {
			ibm.infoBoxes.Remove (this);
			ibm.UpdateBackground ();
		}

		if(onDisappearMethod != null)
			onDisappearMethod ();
	}
}
