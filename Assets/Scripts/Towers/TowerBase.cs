using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class TowerBase : MonoBehaviour, IPointerClickHandler, IFocusable
{

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

    void Start()
    {
        originalPos = transform.position;

        if (tag == "SuperBase")
        {
            nonClickedPickaxeRenderer.sprite = defaultSuperSprite;
            clickedPickaxeRenderer.sprite = clickedSuperSprite;
        }
        else
        {
            nonClickedPickaxeRenderer.sprite = defaultSprite;
            clickedPickaxeRenderer.sprite = clickedSprite;
        }

        ValueStore.Instance.towerManagerInstance.TowerDeployed += OnTowerDeployed;
    }

    public void SetLayer(string sortingLayer)
    {
        foreach (var item in GetComponentsInChildren<SpriteRenderer>(true))
        {
            item.sortingLayerName = sortingLayer;
        }
    }

    public void SetState(TowerBaseState s)
    {
        if (s == TowerBaseState.Clicked)
        {
            clicked.SetActive(true);
            nonClicked.SetActive(false);
            anim.SetBool("Clicked", true);
        }
        else if (s == TowerBaseState.NonClicked)
        {
            clicked.SetActive(false);
            nonClicked.SetActive(true);
            anim.SetBool("Clicked", false);
        }
    }

    private void OnTowerDeployed(Tower t)
    {

    }

    public void CreateTower()
    {
        ValueStore.Instance.towerManagerInstance.CreateTowerIfEnoughMoney(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ValueStore.sharedInstance.OnClick(ClickType.TowerBase, gameObject);

        // ValueStore.sharedInstance.towerManagerInstance.OnTowerBaseClicked(this);
    }

    private void OnDestroy()
    {
        ValueStore.Instance.towerManagerInstance.TowerDeployed -= OnTowerDeployed;
    }

    public void Focus()
    {
        SetState(TowerBaseState.Clicked);
    }

    public void UnFocus()
    {
        SetState(TowerBaseState.NonClicked);
    }
}

public enum TowerBaseState
{
    Clicked,
    NonClicked
}