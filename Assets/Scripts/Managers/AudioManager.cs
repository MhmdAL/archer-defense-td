using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public AudioClip explosionSound, arrowSound, bipSound;

	public void PlaySound(string name, Vector3 pos){
		if (name == "Explosion") {
			AudioSource.PlayClipAtPoint (explosionSound, pos);
		} else if (name == "Arrow") {
			AudioSource.PlayClipAtPoint (arrowSound, pos);
		} else if (name == "Bip") {
			AudioSource.PlayClipAtPoint (bipSound, pos);
		}
	}
		
}
