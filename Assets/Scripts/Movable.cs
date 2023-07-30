using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public float moveSpeed = 5f;

    public bool hasFocus = false;

    private void Update()
    {
        if (hasFocus)
        {
            // Get input from WASD keys
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate movement direction
            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

            // Move the game object
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
