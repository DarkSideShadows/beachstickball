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

    public float hitForce = 10f;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) // volleybaoo has "Ball" tag
        {
            BallController ballController = other.GetComponent<BallController>(); // get BallController component from ball

            if (ballController != null)
            {
                Vector3 hitDirection = (other.transform.position - transform.position).normalized; // find direction from stick to the ball
                hitDirection.y = 0.5f; // ball moves slightly upward when hit

                ballController.ApplyHitForce(hitDirection, hitForce); // apply force to ball
            }
        }
    }
}