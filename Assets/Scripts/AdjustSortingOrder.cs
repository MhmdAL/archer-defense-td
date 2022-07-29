using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdjustSortingOrder : MonoBehaviour
{
    private List<SortingOrderOffsetData> _itemsToBeSorted;

    private void Start()
    {
        _itemsToBeSorted = new List<SortingOrderOffsetData>();

        foreach (var item in transform.GetComponentsInChildren<SpriteRenderer>(true).ToList())
        {
            _itemsToBeSorted.Add(new SortingOrderOffsetData
            {
                SpriteRenderer = item,
                SortingOrderOffset = item.sortingOrder
            });

            print(item.name);
        }

        foreach (var item in transform.GetComponentsInChildren<Canvas>(true).ToList())
        {
            _itemsToBeSorted.Add(new SortingOrderOffsetData
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
        foreach (var item in _itemsToBeSorted)
        {
            if (item.SpriteRenderer != null)
            {
                item.SpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -150) + item.SortingOrderOffset;
            }
            else
            {
                item.Canvas.sortingOrder = Mathf.RoundToInt(transform.position.y * -150) + item.SortingOrderOffset;
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