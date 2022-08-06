using UnityEngine;
using System.Collections;

public class LevelPopUpText : MonoBehaviour {

	public GameObject bgImage;

	public void OnAnimationEnd(){
		RectTransform t = (RectTransform)transform;
		t.position = bgImage.transform.position;
		ValueStore.Instance.AnchorsToCorners (t);

	}
}
