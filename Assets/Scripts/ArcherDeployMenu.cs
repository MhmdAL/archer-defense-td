using UnityEngine;
using System.Collections;

public class ArcherDeployMenu : MonoBehaviour {

	[HideInInspector]
	public TowerBase tb;

	[HideInInspector]	public GameObject objToFollow;
	private Bounds objBounds;

	[HideInInspector]	public float verticalOffset;

	private Transform myTransform;

	private Camera mainCamera;

	void Start(){
		myTransform = transform;
		mainCamera = ValueStore.sharedInstance.mainCamera;
		//TODO: Stop using getcomponent 
		Renderer[] renderersToFollow = objToFollow.GetComponentsInChildren<Renderer>();
		objBounds = renderersToFollow[0].bounds;
		foreach (Renderer item in renderersToFollow) {
			objBounds.Encapsulate (item.bounds);
		}

		PinchZoom.CameraSizeChanged += OnCameraSizeChange;
	}

	void Update(){
		float scale = 20f / mainCamera.orthographicSize + 0.3f;
		myTransform.localScale = new Vector3(scale, scale, 0);

		FollowObject ();
	}

	public void OnClick(){
		tb.CreateTower ();
	}

	public void FollowObject(){
		if (objToFollow != null) {
			Vector3 pos = mainCamera.WorldToScreenPoint (objToFollow.transform.position + new Vector3(0, objBounds.extents.y * 1.5f, 0))
				+ new Vector3(0, ( ( (RectTransform)myTransform).rect.height * myTransform.localScale.y) / 2, 0);

			myTransform.position = pos;
		}
	}

	public void OnCameraSizeChange(){
		//foreach (var item in objToFollow.GetComponentsInChildren<Renderer>()) {
		//	objBounds.Encapsulate (item.bounds);
		//}
	}
}
