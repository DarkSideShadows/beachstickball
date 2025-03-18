using UnityEngine;

/* handles animation transitions
 * walk -> idle : speed < 0.1f
 * idle -> walk : speed > 0.1f
 */
public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    CharacterController controller;
    PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // determine if using arrow keys or WASD
        float horizontal = Input.GetAxis(playerController.horizontalInput);
        float vertical = Input.GetAxis(playerController.verticalInput);

        float speed = new Vector3(horizontal, 0, vertical).magnitude;
        animator.SetFloat("speed", speed);
    }
}