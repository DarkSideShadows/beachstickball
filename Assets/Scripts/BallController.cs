using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* volleball should be like a light beachball
 * this script ensures physics interactions
 * and controls ball movement with forces based on player interaction
 */
public class BallController : MonoBehaviour
{
    public float hitForce = 5f;
    public float bounceForce = 5f;
    public float drag = 0.5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // re-enable gravity
        rb.drag = drag; // smoother, slower movement
        rb.angularDrag = drag; // natural spin
    }

    // when ball is hit by player or object
    public void ApplyHitForce(Vector3 direction, float hitStrength)
    {
        rb.AddForce(direction * hitStrength, ForceMode.Impulse);
    }

    // collision logic (ball bounces off ground)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1f)
        {
            // apply bounce effect
            Vector3 bounce = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            rb.velocity = bounce * bounceForce;
        }
    }
}