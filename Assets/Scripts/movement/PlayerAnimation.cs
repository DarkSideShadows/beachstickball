using Unity.Netcode;
using UnityEngine;

/* handles animation transitions:
 *
 * walk -> idle : speed < 0.1f
 * idle -> walk : speed > 0.1f
 *
 * idle/walk -> jump : isJumping = true
 * jump -> idle/walk : isJumping = false
 */
public class PlayerAnimation : NetworkBehaviour
{
    Animator animator;
    PlayerController playerController;
    StickController stickController;

    // track synced animation states - synchronizing animations over the network (animate non-owned prefabs)
    private float syncedSpeed;
    private bool syncedIsJumping;
    private bool syncedIsSwinging;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        stickController = GetComponentInChildren<StickController>();
    }

    void Update()
    {
        // prevent animations on non-owner object
        if (!playerController.IsOwner) return; 

        // read local input
        float horizontal = Input.GetAxis(playerController.horizontalInput); // determine if using arrow keys or WASD
        float vertical = Input.GetAxis(playerController.verticalInput);     // determine if using arrow keys or WASD
        float speed = new Vector3(horizontal, 0, vertical).magnitude;       // determine walk or idle
        bool isJumping = playerController.isJumping;
        bool isSwinging = stickController.isSwinging;

        // update local animation
        animator.SetFloat("speed", speed);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("swing", isSwinging);

        // sync to other machines - send to server, server broadcasts to all clients
        SendAnimUpdateServerRpc(speed, isJumping, isSwinging);
    }

    [ServerRpc]
    void SendAnimUpdateServerRpc(float speed, bool isJumping, bool isSwinging)
    {
        UpdateAnimClientRpc(speed, isJumping, isSwinging);
    }

    [ClientRpc]
    void UpdateAnimClientRpc(float speed, bool isJumping, bool isSwinging)
    {
        if (IsOwner) return;          // skip owner - already animated locally
        if (animator == null) return; // wait until Start() assigns animator

        syncedSpeed = speed;
        syncedIsJumping = isJumping;
        syncedIsSwinging = isSwinging;

        animator.SetFloat("speed", syncedSpeed);
        animator.SetBool("isJumping", syncedIsJumping);
        animator.SetBool("swing", syncedIsSwinging);
    }
}