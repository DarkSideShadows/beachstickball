using UnityEngine;
using Unity.Netcode;

/* manages network synchronization of the player's position 
 * client requires permission from server to move
 * why do we need to do this?
 * every player runs on at least two machines, (character is displayed on both host and at least one client)
 * because of this, we need to ensure both machines have correct information about the object on screen
 *
 * NOTE:
 * map custom input in unity editor: edit > project settings > input manager > new axes for "Horizontal" and "Vertical"
 */

public class PlayerController : NetworkBehaviour
{
    public Transform characterModel;
    private CharacterController controller;

    // tunables for player movement
    public string horizontalInput = "Horizontal";
    public string verticalInput  = "Vertical";
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.8f;
    public float jumpHeight = 1f;
    public bool isJumping { get; private set; }

    // constants for adjustments
    private Vector3 characterModelOffset = new Vector3(0f, 1f, 0f);  // offset to move the character model downwards
    private float verticalVelocity = 0f;

    // audio
    public AudioSource audioSource;
    public AudioClip jumpSound;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.center = characterModel.position - transform.position + characterModelOffset; // center of collider to center of character model
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; // ensure only local client can control player movement
        HandlePlayerMovement();
    }

    // handle local input, send to server for processing
    void HandlePlayerMovement()
    {
        // get input from horizontal and vertical axes
        float horizontal = Input.GetAxisRaw(horizontalInput); // AD or arrow key
        float vertical = Input.GetAxisRaw(verticalInput);     // WS or arrow key
        Vector3 input = new Vector3(horizontal, 0, vertical).normalized;

        // get camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0; cameraRight.y = 0;             // don't include Y-axis (vertical component)
        cameraForward.Normalize(); cameraRight.Normalize(); // normalize to not get scaled movement

        // request to move player
        Vector3 moveDirection = (cameraForward * input.z + cameraRight * input.x).normalized;
        isJumping = Input.GetKeyDown(KeyCode.Space);
        RequestMoveServerRpc(moveDirection, isJumping);
    }

    [ServerRpc]
    void RequestMoveServerRpc(Vector3 moveDirection, bool isJumping, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return; // only server processes movement
        ApplyMovement(moveDirection, isJumping);
    }

    // move the character prefab on the server
    void ApplyMovement(Vector3 moveDirection, bool isJumping)
    {
        float delta = (float)NetworkManager.Singleton.ServerTime.FixedDeltaTime; // fixed server time
        Vector3 finalMovement = moveDirection * moveSpeed * delta;

        // handle gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // keep player grounded
            if (isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                audioSource.PlayOneShot(jumpSound);
                isJumping = true; // player just jumped
            }
            else
            {
                isJumping = false; // player just landed
            }
        }
        else
        {
            verticalVelocity += gravity * NetworkManager.Singleton.ServerTime.FixedDeltaTime;
        }
        finalMovement.y = verticalVelocity * NetworkManager.Singleton.ServerTime.FixedDeltaTime;

        controller.Move(finalMovement);      // move character in world
        ApplyRotation(moveDirection, delta); // align character model with move direction
    }

    void ApplyRotation(Vector3 moveDirection, float delta)
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
        }
    }
}