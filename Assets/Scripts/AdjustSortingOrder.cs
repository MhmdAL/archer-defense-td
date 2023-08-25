using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class AdjustSortingOrder : MonoBehaviour
{
    [SerializeField]
    private List<SortingOrderOffsetData> itemsToBeSorted;

    private void Awake()
    {
        LoadCurrentObjectSprites();
    }

    private void OnEnable()
    {
        LoadCurrentObjectSprites();
    }

    private void LoadCurrentObjectSprites()
    {
        if (itemsToBeSorted.Any())
        {
            return;
        }

        itemsToBeSorted = new List<SortingOrderOffsetData>();

        foreach (var item in transform.GetComponentsInChildren<SpriteRenderer>(true).ToList())
        {
            itemsToBeSorted.Add(new SortingOrderOffsetData
            {
                SpriteRenderer = item,
                SortingOrderOffset = item.sortingOrder
            });
        }

        foreach (var item in transform.GetComponentsInChildren<Canvas>(true).ToList())
        {
            itemsToBeSorted.Add(new SortingOrderOffsetData
            {
                Canvas = item,
                SortingOrderOffset = item.sortingOrder
            });
        }
    }

    private void Update()
    {
        Adjust();
    }

    public void Adjust()
    {
        foreach (var item in itemsToBeSorted)
        {
            if (item.SpriteRenderer != null)
            {
                item.SpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -500) + item.SortingOrderOffset;
            }
            else
            {
                item.Canvas.sortingOrder = Mathf.RoundToInt(transform.position.y * -500) + item.SortingOrderOffset;
            }
        }
    }
}

[Serializable]
public class SortingOrderOffsetData
{
    public SpriteRenderer SpriteRenderer;
    public Canvas Canvas;
    public int SortingOrderOffset;
}