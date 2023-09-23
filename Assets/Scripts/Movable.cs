using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IFocusable))]
[RequireComponent(typeof(IMoving))]
public class Movable : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public Transform body;

    private IFocusable _focusable;
    private IMoving _movable;

    private bool _isMoving;

    private void Awake()
    {
        _focusable = GetComponent<IFocusable>();
        _movable = GetComponent<IMoving>();
    }

    private void Update()
    {
        animator.SetFloat("movespeed", rigidbody2D.velocity.magnitude);

        if (_focusable.HasFocus)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

            if (Mathf.Abs(moveDirection.x) > 0)
            {
                AdjustSpriteDirection(moveDirection.x > 0 ? true : false);
            }

            var force = moveDirection * _movable.MoveSpeed.Value * Time.deltaTime;

            var isMoving = force.sqrMagnitude > 0;

            UpdateMovementState(isMoving);

            rigidbody2D.AddForce(force);
        }
        else if (!_focusable.HasFocus && _isMoving) // player unfocused but still moving. Should stop moving
        {
            UpdateMovementState(false);
        }
    }

    private void UpdateMovementState(bool isMoving)
    {
        if (!_isMoving && isMoving)
        {
            _movable.OnMovementStarted();
        }
        else if (_isMoving && !isMoving)
        {
            _movable.OnMovementEnded();
        }

        _isMoving = isMoving;
    }

    private void AdjustSpriteDirection(bool lookRight)
    {
        var angle = lookRight ? 180 : 0;
        body.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
