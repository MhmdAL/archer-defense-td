using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using EPOOutline;

public abstract class Draggable : MonoBehaviour, IPointerClickHandler
{
    private bool _isDragged;
    private Vector3 _initialPos;
    private Outlinable _outlinable;

    private Tower _target;

    private bool _isConsumed;

    private void Awake()
    {
        _outlinable = GetComponent<Outlinable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isDragged)
        {
            _initialPos = Input.mousePosition.ToWorldPosition(Camera.main);
            _initialPos.z = 0;

            _isDragged = true;
        }
        else
        {
            _isDragged = false;

            var pos = Input.mousePosition.ToWorldPosition(Camera.main);
            pos.z = 0;

            transform.position = pos;

            if (_target != null)
            {
                Apply(_target);
            }
        }
    }

    protected virtual void Apply(Tower target)
    {
        _isConsumed = true;

        Debug.Log("Apply effect to: " + target.name);

        transform.DOScale(0.01f, 0.2f).onComplete = () =>
        {
            gameObject.SetActive(false);

            if (this != null)
            {
                Destroy(gameObject, 2f);
            }
        };
    }

    private void Update()
    {
        if (_isDragged && !_isConsumed)
        {
            var newPos = Input.mousePosition.ToWorldPosition(Camera.main);
            newPos.z = 0;

            transform.position = newPos;

            var hit = Physics2D.Raycast(newPos, Vector2.one, 1000, LayerMask.GetMask("Archer"));

            if (hit && hit.transform.TryGetComponent<Tower>(out _target))
            {
                _outlinable.OutlineParameters.Enabled = true;

                transform.DOScale(25, 0.4f);
            }
            else
            {
                _target = null;

                _outlinable.OutlineParameters.Enabled = false;

                transform.DOScale(10, 0.2f);
            }
        }
    }
}
