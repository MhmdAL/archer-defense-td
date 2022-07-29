using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class EnemyBoss4 : Monster {

	[Header("Prefabs")]
	public EnemyBoss4Circle circle;

	private EnemyBoss4Circle myCircle;

	[Header("Disability Options")]
	public float disableShockwaveCooldown;
	public float disableShockwaveDuration;
	public float disableShockwaveRange;

	CooldownTimer shockwaveCooldown;

	protected override void Awake(){
		base.Awake ();

		shockwaveCooldown = new CooldownTimer (disableShockwaveCooldown);
	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (shockwaveCooldown.GetCooldownRemaining () <= 0) {
			anim.SetBool ("PlayShockwave", true);
			shockwaveCooldown.ResetTimer (disableShockwaveCooldown);
		}

	}

	public void StopShockwaveAnimation(){
		anim.SetBool ("PlayShockwave", false);
	}

	public void Shockwave(){
		myCircle = Instantiate (circle, transform.position, Quaternion.identity) as EnemyBoss4Circle;
		myCircle.disableShockwaveDuration = this.disableShockwaveDuration;
		StartCoroutine (ExpandCircle (myCircle));
	}

	IEnumerator ExpandCircle(EnemyBoss4Circle g){
		for (int i = 0; i < 10; i++) {
			g.transform.localScale += new Vector3 (disableShockwaveRange / 5, disableShockwaveRange / 5, 0);
			yield return new WaitForSeconds (0.04f);
		}
		Destroy (g.gameObject);
	}

	void OnDestroy(){
		if (myCircle != null) {
			Destroy (myCircle.gameObject);
		}
	}
}