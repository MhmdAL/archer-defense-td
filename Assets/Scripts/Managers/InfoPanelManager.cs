using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class InfoPanelManager : MonoBehaviour {

	public List<InfoPanel> infoPanels = new List<InfoPanel>(4);

	private List<Vector3> panelPositions = new List<Vector3>(4);

	[Header("Options")]
	public float delayTillDeath;

	void Awake(){
		float offset = 50;
		int width = Screen.width;
		int height = Screen.height;

		panelPositions.Add(new Vector3 (width / 4 + offset, height * 3 / 4 - offset, 0));
		panelPositions.Add(new Vector3 (width * 3 / 4 - offset, height * 3 / 4 - offset, 0));
		panelPositions.Add(new Vector3 (width / 4 + offset, height / 4 + offset, 0));
		panelPositions.Add(new Vector3 (width * 3 / 4 - offset, height / 4 + offset, 0));
	}

	void Start(){
		if (infoPanels.Count == 0)
			return;
		
		if (infoPanels.Capacity > 4)
			infoPanels.Capacity = 4;

		for (int i = 0; i < infoPanels.Capacity; i++) {
			if (infoPanels [i] == null)
				continue;
			
			if (infoPanels.Count == 1) {
				infoPanels [i].transform.parent.position = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
				return;
			}

			infoPanels [i].transform.parent.position = panelPositions [i];
		}
	}

	public void ShowPanels(){
		foreach (var item in infoPanels) {
			item.transform.parent.gameObject.SetActive (true);
		}
	}
}
