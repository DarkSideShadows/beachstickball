using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using beachstickball;

/*
* map custom input in unity editor
* edit > project settings > input manager > new axes for "Horizontal" and "Vertical"
*
* CONTROLS PLAYER MOVEMENT - manages local player's input and movement
* works for both player1 (host) and player2 (client)
*/

/* manages network synchronization of the player's position
 * client requires permission from host to move
 *
 * why do we need to do this?
 * every player runs on at least two machines,
 * because of this, we need to ensure both machines have correct information about the object on screen
 * only one player controls how the object moves
 */

public class PlayerController : NetworkBehaviour
{
    public Transform characterModel; // reference to flamingo/frog character model

    // tunables for player movement
    public float moveSpeed = 5f; // default, but manual change:
    // ^ i found that client moved much faster than host
    // in the editor, i manually changed the move speed to be higher in hosts
    // to match the client move speed (temporary fix, but works)
    public float jumpHeight = 3f;
    public float gravity = -9.8f;
    public string horizontalInput = "Horizontal";
    public string verticalInput  = "Vertical";
    public float rotationSpeed = 10f; // speed of character rotation when moving

    // internal variables
    private CharacterController controller;
    private Vector3 movement;
    private float verticalVelocity;

    // constants for adjustments
    private Vector3 characterModelOffset = new Vector3(0f, 1f, 0f);  // offset to move the character model downwards

    // audio
    public AudioSource audioSource;
    public AudioClip jumpSound;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.center = characterModel.position - transform.position + characterModelOffset; // center of collider to center of character model
    }

    void Update()
    {
        // ensure only local client can control player movement
        if (!IsOwner) return;
        //ulong localClientId = NetworkManager.Singleton.LocalClientId;
        //Debug.Log("Local Client ID: " + localClientId);
        HandlePlayerMovement();
    }

    void HandlePlayerMovement()
    {
        // get input from horizontal and vertical axes
        float horizontal = Input.GetAxisRaw(horizontalInput);
        float vertical = Input.GetAxisRaw(verticalInput);
        Vector3 movementInput = new Vector3(horizontal, 0, vertical).normalized;

        //Debug.Log($"[{OwnerClientId}] Movement Input: {horizontal}, {vertical}");

        if (movementInput.magnitude > 0 || Input.GetButtonDown("Jump"))
        {
            RequestMoveServerRpc(movementInput, Input.GetButtonDown("Jump"));
        }
    }

    [ServerRpc]
    void RequestMoveServerRpc(Vector3 moveDirection, bool isJumping, ServerRpcParams rpcParams = default)
    {
        // ensure only server processes movement
        if (!IsServer) return;
        ApplyMovement(moveDirection, isJumping);
    }

    // move the character prefab
    void ApplyMovement(Vector3 moveDirection, bool isJumping)
    {
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
        Vector3 finalMoveDirection = (cameraForward * moveDirection.z + cameraRight * moveDirection.x).normalized; // normalize to prevent faster diagonal movement
        Vector3 finalMovement = finalMoveDirection * moveSpeed * Time.deltaTime; // apply move speed

        // jumping and gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // keep the player grounded
            if(isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                audioSource.PlayOneShot(jumpSound); // play jump noise
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        finalMovement.y = verticalVelocity * Time.deltaTime; // apply gravity

        // move player while handling collisions
        controller.Move(finalMovement);

        // align character model with move direction
        ApplyRotation(finalMoveDirection);
    }

    void ApplyRotation(Vector3 moveDirection)
    {
        // align the character capsule to the root of the character model
        if (characterModel != null)
        {
            Vector3 modelPosition = characterModel.position;
            controller.transform.position = new Vector3(modelPosition.x, controller.transform.position.y, modelPosition.z);
        }

        // rotate the character based on movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}