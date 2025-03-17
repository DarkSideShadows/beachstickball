using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* trigger swing animation
*  interact with the volleyball
*/
public class StickSwing : MonoBehaviour
{
    // stick swing animatino
    public Transform stickTransform; // stick
    public float swingSpeed = 10f;
    public float swingAngle = 45;
    private bool isSwinging = false;
    private Quaternion originalRotation;

    public float hitForce = 5f;

    void Start()
    {
        originalRotation = stickTransform.localRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isSwinging) // X to swing stick
        {
            StartCoroutine(SwingStick());
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
                Vector3 hitDirection = ball.transform.position - stickTransform.position;
                hitDirection = hitDirection.normalized; // make direction unit vector
                hitDirection.y = 0.5f; // make the ball go slightly upwards after the hit

                // apply force to ball with the new direction
                ballController.ApplyHitForce(hitDirection, hitForce);
            }
        }
    }
}