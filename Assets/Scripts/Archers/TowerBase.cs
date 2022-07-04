using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class TowerBase : MonoBehaviour, IPointerClickHandler {

	[Header("Clicked/NonClicked Objects")]
	public GameObject clicked;
	public GameObject nonClicked;

	public SpriteRenderer clickedPickaxeRenderer;
	public SpriteRenderer nonClickedPickaxeRenderer;

	public Sprite defaultSprite;
	public Sprite clickedSprite;
	public Sprite defaultSuperSprite;
	public Sprite clickedSuperSprite;

	[HideInInspector]
	public Vector3 originalPos; 

	public Animator anim;

	void Start(){
		originalPos = transform.position;

		AdjustSortingOrder ();

		if (tag == "SuperBase") {
			nonClickedPickaxeRenderer.sprite = defaultSuperSprite;
			clickedPickaxeRenderer.sprite = clickedSuperSprite;
		} else {
			nonClickedPickaxeRenderer.sprite = defaultSprite;
			clickedPickaxeRenderer.sprite = clickedSprite;
		}
	}

	void Update(){
		if (ValueStore.sharedInstance.lastClickType == ClickType.TowerBase) {
			if (gameObject == ValueStore.sharedInstance.lastClicked) {
				SetState (TowerBaseState.Clicked);
			} else {
				SetState (TowerBaseState.NonClicked);
			}
		} else {
			SetState (TowerBaseState.NonClicked);
		}
	}

	private void AdjustSortingOrder(){
		// adjust sorting order of the tower components
		foreach (var item in GetComponentsInChildren<SpriteRenderer>(true).ToList()) {
			item.sortingOrder += Mathf.RoundToInt ((transform.position.y) * -150);	
		}
	}

	public void SetLayer(string sortingLayer){
		foreach (var item in GetComponentsInChildren<SpriteRenderer>(true)) {
			item.sortingLayerName = sortingLayer;
		}
	}

	public void SetState(TowerBaseState s){
		if (s == TowerBaseState.Clicked) {
			clicked.SetActive (true);
			nonClicked.SetActive (false);
			anim.SetBool ("Clicked", true);
		} else if (s == TowerBaseState.NonClicked) {
			clicked.SetActive (false);
			nonClicked.SetActive (true);
			anim.SetBool ("Clicked", false);
		}
	}

	public void CreateTower(){
		ValueStore.sharedInstance.towerManagerInstance.CreateTower (this);
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		ValueStore.sharedInstance.OnClick (ClickType.TowerBase, gameObject);
	}
}

public enum TowerBaseState
{
	Clicked,
	NonClicked
}