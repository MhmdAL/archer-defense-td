using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IFocusable))]
[RequireComponent(typeof(IMovable))]
public class Movable : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public Transform body;

    private IFocusable _focusable;
    private IMovable _movable;

    private void Awake()
    {
        _focusable = GetComponent<IFocusable>();
        _movable = GetComponent<IMovable>();
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

            rigidbody2D.AddForce(moveDirection * _movable.MoveSpeed.Value);
        }
    }

    private void AdjustSpriteDirection(bool lookRight)
    {
        var angle = lookRight ? 180 : 0;
        body.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
