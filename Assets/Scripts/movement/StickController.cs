using System.Collections;
using UnityEngine;

/* trigger swing animation interact with the volleyball */
public class StickController : MonoBehaviour
{
    private PlayerController playerController;

    // external tunables for stick animation
    public Transform stickTransform;
    public float swingSpeed = 10f;
    public float swingAngle = 45;
    public float hitForce = 5f;
    public bool isSwinging { get; private set; }

    // constants
    private float courtWidth = 2f;
    private Quaternion originalRotation;

    // audio
    public AudioSource audioSource;
    public AudioClip swingSound;   // slash sound effect
    public AudioClip ballHitSound; // hit ball success

    void Start()
    {
        originalRotation = stickTransform.localRotation;
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        // remove controls for non-owned objects
        if (playerController == null || !playerController.IsOwner) return;

        if (Input.GetKeyDown(KeyCode.X) && !isSwinging)
        {
            StartCoroutine(SwingStick());           // handle stick motion
            StartCoroutine(ResetSwingAnimFlag());   // handle arm animation
            audioSource.PlayOneShot(swingSound);
        }
    }

    IEnumerator ResetSwingAnimFlag()
    {
        yield return new WaitForSeconds(0.2f); // duration of punch animation
        isSwinging = false;                    // animation ends early (allows transition to other animation)
    }

    IEnumerator SwingStick()
    {
        isSwinging = true; // start swinging animation (prevent multiple from happening at the same time)

        /// swing forward ///
        float elapsedTime = 0f;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(-swingAngle, 0, 0);
        /* ^ explanation of variables:
        * originalRotation stores stick's starting rotation saved in Start()
        * Quaternion.Euler(-swingAngle, 0, 0) rotates stick forward by swingAngle (-45 degrees)
        * targetRotation saves the new rotation that we want to reach
        */

        while(elapsedTime < 0.4f)
        {
            stickTransform.localRotation = Quaternion.Lerp(originalRotation, targetRotation, elapsedTime * swingSpeed);
            elapsedTime += Time.deltaTime;
            /* ^ explanation of variables:
            * Lerp (linear interpolation) gradually moves rotation from originalRotation -> targetRotation
            * elapsedTime counts and ensures it takes 0.4 seconds to reach targetPosition
            */
            yield return null;
        }
        stickTransform.localRotation = targetRotation; // ensure final position (swing forward)

        /// swing back (return stick to original position) ///
        elapsedTime = 0f;
        while(elapsedTime < 0.4f)
        {
            stickTransform.localRotation = Quaternion.Lerp(targetRotation, originalRotation, elapsedTime * swingSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        stickTransform.localRotation = originalRotation; // ensure final position (original position)

        isSwinging = false; // finish swinging
    }

    // interact with volleyball
    void OnTriggerEnter(Collider ball)
    {
        if (ball.CompareTag("Ball")) // volleybaoo has "Ball" tag
        {
            BallController ballController = ball.GetComponent<BallController>(); // get BallController component from ball

            if (ballController != null && isSwinging) // THEN, HIT THE BALL
            {
                // play ball hit sound
                audioSource.PlayOneShot(ballHitSound);

                // calculate hit direction
                Vector3 targetDirection = GetHitDirection(); // pseudo-random place to ensure landing within the court
                Vector3 hitDirection = targetDirection.normalized; // make direction unit vector

                // apply force to ball with the new direction
                ballController.ApplyHitForceServerRpc(hitDirection, hitForce);
            }
        }
    }

    Vector3 GetHitDirection()
    {
        string playerTag = gameObject.tag; // "Player 1" or "Player 2"

        // define the court boundaries
        float minX = -courtWidth / 2 + 0.5f;
        float maxX =  courtWidth / 2 - 0.5f;
        float targetX = Random.Range(minX, maxX); // consistent randomness per player
        float targetZ = (playerTag == "Player 1") ? -1f : 1f; // ball always goes towards the opponent's side

        return new Vector3(targetX, 1f, targetZ);
    }
}