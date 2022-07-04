using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AnchorSetter : MonoBehaviour {

	void Start(){
		RectTransform t = transform.GetChild (0) as RectTransform;
		RectTransform ct = t.GetChild(0) as RectTransform;
		RectTransform cct = ct.GetChild(0) as RectTransform;
		RectTransform ccct = cct.GetChild(0) as RectTransform;

		RectTransform pt = transform as RectTransform;
		RectTransform ppt = transform.parent as RectTransform;
		RectTransform pppt = transform.parent.parent as RectTransform;

		if(t == null || pt == null) 
			return;
		// First Child
		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
		// Second Child
		newAnchorsMin = new Vector2(ct.anchorMin.x + ct.offsetMin.x / t.rect.width,
			ct.anchorMin.y + ct.offsetMin.y / t.rect.height);
		newAnchorsMax = new Vector2(ct.anchorMax.x + ct.offsetMax.x / t.rect.width,
			ct.anchorMax.y + ct.offsetMax.y / t.rect.height);

		float xFactor = t.rect.width;
		float yFactor = t.rect.height;
		Vector2 factor = new Vector2 (xFactor, yFactor);

		//newAnchorsMin.Scale(factor);
		//newAnchorsMax.Scale (factor);

		ct.anchorMin = newAnchorsMin;
		ct.anchorMax = newAnchorsMax;
		ct.offsetMin = ct.offsetMax = new Vector2(0, 0);
		// Third Child
		newAnchorsMin = new Vector2(cct.anchorMin.x + cct.offsetMin.x / ct.rect.width,
			cct.anchorMin.y + cct.offsetMin.y / ct.rect.height);
		newAnchorsMax = new Vector2(cct.anchorMax.x + cct.offsetMax.x / ct.rect.width,
			cct.anchorMax.y + cct.offsetMax.y / ct.rect.height);

		xFactor = ct.rect.width / t.rect.width;
		yFactor = ct.rect.height / t.rect.height;
		factor = new Vector2 (xFactor, yFactor);

		//newAnchorsMin.Scale(factor);
		//newAnchorsMax.Scale (factor);

		//cct.anchorMin = newAnchorsMin;
		//cct.anchorMax = newAnchorsMax;
		//cct.offsetMin = cct.offsetMax = new Vector2(0, 0);
		// Fourth Child
		newAnchorsMin = new Vector2(ccct.anchorMin.x + ccct.offsetMin.x / cct.rect.width,
			ccct.anchorMin.y + ccct.offsetMin.y / cct.rect.height);
		newAnchorsMax = new Vector2(ccct.anchorMax.x + ccct.offsetMax.x / cct.rect.width,
			ccct.anchorMax.y + ccct.offsetMax.y / cct.rect.height);

		xFactor = cct.rect.width / ct.rect.width;
		yFactor = cct.rect.height / ct.rect.height;
		factor = new Vector2 (xFactor, yFactor);

		//newAnchorsMin.Scale(factor);
		//newAnchorsMax.Scale (factor);

		//ccct.anchorMin = newAnchorsMin;
		//ccct.anchorMax = newAnchorsMax;
		//ccct.offsetMin = ccct.offsetMax = new Vector2(0, 0);
	}

	private void AnchorsToCorners(){
		RectTransform t = transform as RectTransform;
		RectTransform pt = transform.parent as RectTransform;

		if(t == null || pt == null) return;

		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}
}
