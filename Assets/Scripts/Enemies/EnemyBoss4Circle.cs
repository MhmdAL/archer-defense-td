using UnityEngine;
using System.Collections;

public class EnemyBoss4Circle : MonoBehaviour {

	[HideInInspector]	public float disableShockwaveDuration;

	void OnTriggerEnter2D(Collider2D c){
		Tower t = c.GetComponent<Tower> ();
		if (t != null) {
			if(t.active){
				t.AddModifier (new Modifier (Name.EnemyBoss4_Disablility, Type.Status,
					disableShockwaveDuration, ApplyDisableModifier, DeApplyDisableModifier), StackOperation.Additive, 1);
			}
		}
	}

	public void ApplyDisableModifier(IModifiable m){
		Tower t = (Tower)m;
		t.archerTowerRenderer.color = Color.black;
		t.active = false;
	}

	public void DeApplyDisableModifier(IModifiable m){
		Tower t = (Tower)m;
		t.active = true;
		t.archerTowerRenderer.color = Color.white;
	}
}
