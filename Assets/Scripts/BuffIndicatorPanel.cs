using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class BuffIndicatorPanel : MonoBehaviour {

	public BuffIndicator buffIndicatorPrefab;

	[HideInInspector]	public List<BuffIndicator> indicators = new List<BuffIndicator>();

	public void AddIndicator(BuffIndicatorType b, CooldownTimer cd = null, MyTimer t = null){
		if (indicators.FirstOrDefault (x => x.type == b) == null) {
			BuffIndicator bi = Instantiate (buffIndicatorPrefab) as BuffIndicator;
			bi.transform.SetParent (transform);
			bi.type = b;
			bi.owner = this;

			if (cd != null) {
				bi.cd = cd;
			} 
			
			indicators.Add (bi);
		}
	}

}
