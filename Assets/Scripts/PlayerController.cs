using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* two players first:
* map custom input in unity editor
* edit > project settings > input manager > new axes for "Horizontal" and "Vertical"
*/
public class PlayerController : MonoBehaviour
{
    // constants for player movement
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    public float gravity = -9.8f;

    // variables for player controls (custom inputs)
    public string horizontalInput = "Horizontal";
    public string verticalInput  = "Vertical";

    private CharacterController controller;
    private Vector3 movement;
    private float verticalVelocity; // (vertical + velocity)

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // get input from horizontal and vertical axes
        movement.x = Input.GetAxisRaw(horizontalInput);
        movement.z = Input.GetAxisRaw(verticalInput);
        movement = movement.normalized * moveSpeed * Time.deltaTime; // normalize to prevent faster diagonal movement, apply speed

        // jumping and gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // keep the player grounded
            if(Input.GetButtonDown("Jump")) // default is spacebar
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // move player while handling collisions
        movement.y = verticalVelocity * Time.deltaTime;
        controller.Move(movement);
    }
}