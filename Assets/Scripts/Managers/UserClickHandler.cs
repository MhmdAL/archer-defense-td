using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserClickHandler : MonoBehaviour
{
    public event Action<object> ObjectClicked;

    public Image SelectionImage;

    private CustomStandaloneInputModule _inputModule;
    private Camera _mainCamera;
    private RectTransform _selectionImageRectTransform;
    
    private CanvasScaler _canvasScaler;

    private void Awake()
    {
        _inputModule = FindObjectOfType<CustomStandaloneInputModule>();
        _mainCamera = Camera.main;
        _selectionImageRectTransform = SelectionImage.GetComponent<RectTransform>();

        _canvasScaler = SelectionImage.canvas.GetComponent<CanvasScaler>();
    }

    private Vector3 initialSelectPosition;
    private Vector3 finalSelectPosition;
    private bool isSelecting;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialSelectPosition = Input.mousePosition;
            isSelecting = true;
            SelectionImage.gameObject.SetActive(true);

            // Debug.Log("Mouse down");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("Mouse Up");
            finalSelectPosition = Input.mousePosition;

            isSelecting = false;
            SelectionImage.gameObject.SetActive(false);

            var start = initialSelectPosition.ToWorldPosition(_mainCamera);
            var end = finalSelectPosition.ToWorldPosition(_mainCamera);

            var dir = end - start;

            var hits = new List<Collider2D>();

            if (dir.magnitude < 0.2f)
            {
                hits = new List<Collider2D> { Physics2D.Raycast(start, Vector2.one).collider };
            }
            else
            {
                hits = Physics2D.OverlapAreaAll(start, end).ToList();
            }

            if (!hits.Any() || _inputModule.IsPointerOverGameObject<GraphicRaycaster>()) // is mouse over ui? if so don't process.
            {
                return;
            }

            var filteredList = hits.ToList();

            if (hits.Count > 1)
            {
                var background = hits.FirstOrDefault(x => x.GetComponent<BackgroundScaler>() != null);
                if (background != null)
                {
                    filteredList.Remove(background);
                }
            }

            var focusables = FindObjectsOfType<UnityEngine.Object>()
                .Where(x => x is IFocusable)
                .Select(x => x as IFocusable)
                .ToList();

            // Debug.Log("Found " + focusables.Count + " focusables");
            foreach (var item in focusables)
            {
                item.UnFocus();
            }

            foreach (var hit in filteredList)
            {
                if (hit.TryGetComponent<IFocusable>(out var component))
                {
                    ObjectClicked?.Invoke(component);
                    component.Focus();
                }
            }
        }

        if (isSelecting)
        {
            finalSelectPosition = Input.mousePosition;
            UpdateSelectionBox(_selectionImageRectTransform);
        }
    }

    private void UpdateSelectionBox(RectTransform transform)
    {
        var initialWorldPos = initialSelectPosition.ToWorldPosition(_mainCamera);
        var finalWorldPos = finalSelectPosition.ToWorldPosition(_mainCamera);

        var min = Vector2.Min(initialWorldPos, finalWorldPos);
        var max = Vector2.Max(initialWorldPos, finalWorldPos);
        var minScreen = min.ToScreenPosition(_mainCamera);
        var maxScreen = max.ToScreenPosition(_mainCamera);

        _selectionImageRectTransform.position = initialSelectPosition;

        _selectionImageRectTransform.pivot = new Vector2((Mathf.Sign(initialWorldPos.x - finalWorldPos.x) + 1) / 2,
            (Mathf.Sign(initialWorldPos.y - finalWorldPos.y) + 1) / 2);

        var sizeDelta = (maxScreen - minScreen);

        // TODO: this only works when screen aspect ratio and reference (in canvas) aspect ratio are equal atm.
        sizeDelta.Scale(new Vector2(_canvasScaler.referenceResolution.x / Screen.width, _canvasScaler.referenceResolution.y / Screen.height));

        _selectionImageRectTransform.sizeDelta = sizeDelta;
    }
}
