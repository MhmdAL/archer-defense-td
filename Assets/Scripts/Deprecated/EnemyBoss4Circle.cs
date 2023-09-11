using UnityEngine;
using System.Collections;

public class EnemyBoss4Circle : MonoBehaviour {

	[HideInInspector]	public float disableShockwaveDuration;

	void OnTriggerEnter2D(Collider2D c){
		Tower t = c.GetComponent<Tower> ();
		if (t != null) {
			if(!t.IsDisabled){
				// t.AddModifier (new Modifier (Name.EnemyBoss4_Disablility, Type.Status,
				// 	disableShockwaveDuration, ApplyDisableModifier, DeApplyDisableModifier), StackOperation.Additive, 1);
			}
		}
	}

	public void ApplyDisableModifier(IModifiable m){
		Tower t = (Tower)m;
		t.archerTowerRenderer.color = Color.black;
		t.IsDisabled = true;
	}

	public void DeApplyDisableModifier(IModifiable m){
		Tower t = (Tower)m;
		t.IsDisabled = false;
		t.archerTowerRenderer.color = Color.white;
	}
}
