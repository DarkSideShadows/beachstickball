using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* trigger swing animation
*  interact with the volleyball
*/
public class StickSwing : MonoBehaviour
{
    // stick swing (only for stick)
    public Transform stickTransform;
    public float swingSpeed = 10f;
    public float swingAngle = 45;
    public float hitForce = 5f;
    private bool isSwinging = false;
    private Quaternion originalRotation;

    // animation for swing (including arm)
    public Animator stickAnimator;
    public string swingAnimationTrigger = "swing";

    void Start()
    {
        originalRotation = stickTransform.localRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isSwinging) // X to swing stick
        {
            StartCoroutine(SwingStick());
            stickAnimator.SetBool("swing", true);
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            stickAnimator.SetBool("swing", false);
        }
    }

    IEnumerator SwingStick()
    {
        isSwinging = true; // mark swinging animation (prevent multiple from happening at the same time)

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
                // calculate hit direction
                Vector3 targetDirection = GetHitDirection(); // pseudo-random place to ensure landing within the court
                Vector3 hitDirection = targetDirection.normalized; // make direction unit vector
                hitDirection.y = 1f; // make the ball go slightly upwards after the hit

                // apply force to ball with the new direction
                ballController.ApplyHitForce(hitDirection, hitForce);
            }
        }
    }

    Vector3 GetHitDirection()
    {
        string playerTag = gameObject.tag; // "Player 1" or "Player 2"

        // Define fixed landing positions for Player 1 and Player 2
        Vector3 player1Target = new Vector3(0, 0, -10); // Target position on Player 2's side
        Vector3 player2Target = new Vector3(0, 0, 10); // Target position on Player 1's side

        if(playerTag == "Player 1")
        {
            return player1Target;
        }
        else if(playerTag == "Player 2")
        {
            return player2Target;
        }

        return Vector3.zero; // default
    }
}