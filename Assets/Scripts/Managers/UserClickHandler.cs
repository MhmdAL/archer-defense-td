using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserClickHandler : MonoBehaviour
{
    public event Action<object> ObjectClicked;

    private CustomStandaloneInputModule _inputModule;
    private Camera _mainCamera;

    private void Awake()
    {
        _inputModule  = FindObjectOfType<CustomStandaloneInputModule>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider is null || _inputModule.IsPointerOverGameObject<GraphicRaycaster>())
            {
                return;
            }

            if (hit.collider.TryGetComponent<BackgroundScaler>(out var backgroundComponent))
            {
                ObjectClicked?.Invoke(backgroundComponent);
            }
            else if (hit.collider.TryGetComponent<Tower>(out var towerComponent))
            {
                ObjectClicked?.Invoke(towerComponent);
            }
            else if (hit.collider.TryGetComponent<TowerBase>(out var towerBaseComponent))
            {
                ObjectClicked?.Invoke(towerBaseComponent);
            }
        }
    }
}
