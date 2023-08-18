using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IFocusable))]
public class Movable : MonoBehaviour
{
    public float moveSpeed = 5f;

    public bool hasFocus = false;

    private IFocusable _target;

    private void Awake()
    {
        _target = GetComponent<IFocusable>();
    }

    private void Update()
    {
        if (_target.HasFocus)
        {
            // Get input from WASD keys
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate movement direction
            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

            // Move the game object
            GetComponent<Rigidbody2D>().MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
            // transform.Translate();
        }
    }
}
