using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* two players first:
* map custom input in unity editor
* edit > project settings > input manager > new axes for "Horizontal" and "Vertical"
*
* CONTROLS PLAYER MOVEMENT
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
    private float verticalVelocity; // (movement speed)
    public float rotationSpeed = 10f; // speed of character rotation when moving

    public Transform characterModel; // reference to flamingo/frog character model
    public Vector3 characterModelOffset = new Vector3(0f, 1f, 0f);  // offset to move the character model downwards

    // audio
    public AudioSource audioSource;
    public AudioClip jumpSound;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // center of collider to center of character model
        controller.center = characterModel.position - transform.position + characterModelOffset;
    }

    void Update()
    {
        // get input from horizontal and vertical axes
        float horizontal = Input.GetAxisRaw(horizontalInput);
        float vertical = Input.GetAxisRaw(verticalInput);

        // get camera's rotation
        Vector3 cameraForward = Camera.main.transform.forward; // camera's forward vector
        Vector3 cameraRight = Camera.main.transform.right; // camera's right vector

        // don't include Y-axis (vertical component)
        cameraForward.y = 0;
        cameraRight.y = 0;

        // normalize to not get scaled movement
        cameraForward.Normalize();
        cameraRight.Normalize();

        // adjust player's movement based on camera's orientation
        Vector3 movementDirection = cameraForward * vertical + cameraRight * horizontal;
        movementDirection.Normalize(); // normalize to prevent faster diagonal movement
        movement = movementDirection * moveSpeed * Time.deltaTime;

        // jumping and gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // keep the player grounded
            if(Input.GetButtonDown("Jump")) // default is spacebar
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // play jump noise
                audioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // move player while handling collisions
        movement.y = verticalVelocity * Time.deltaTime;
        controller.Move(movement);

        // align the character capsule to the root of the character model
        if (characterModel != null)
        {
            Vector3 modelPosition = characterModel.position;
            controller.transform.position = new Vector3(modelPosition.x, controller.transform.position.y, modelPosition.z);
        }

        // rotate the character based on movement direction
        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}