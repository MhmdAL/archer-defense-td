using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AdjustSortingOrder : MonoBehaviour
{
    public int CurrentSortingOrder => _canvas != null ? _canvas.sortingOrder : _spriteRenderer.sortingOrder;


    [SerializeField, Header("If not null, the root's y position is used.")]
    private Transform root;

    [SerializeField]
    private int sortingOffset;

    private Canvas _canvas;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        TryGetComponent<Canvas>(out _canvas);
        TryGetComponent<SpriteRenderer>(out _spriteRenderer);
    }

    private void Update()
    {
        Adjust();
    }

    public void Adjust()
    {
        if (_canvas != null)
        {
            _canvas.sortingOrder = root != null ? Mathf.RoundToInt(root.position.y * -500) + sortingOffset 
                : Mathf.RoundToInt(transform.position.y * -500) + sortingOffset;
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = root != null ? Mathf.RoundToInt(root.position.y * -500) + sortingOffset
                : Mathf.RoundToInt(transform.position.y * -500) + sortingOffset;
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