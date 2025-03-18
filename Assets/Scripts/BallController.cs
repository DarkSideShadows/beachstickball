using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* volleball should be like a light beachball
 * this script ensures physics interactions
 * and controls ball movement with forces based on player interaction
 */
public class BallController : MonoBehaviour
{
    public float drag = 0.5f;
    private Rigidbody rb;

    private Vector3 initialPosition;

    // reference to court dimensions
    public float courtWidth = 12.18f;
    public float courtDepth = 21.02f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // re-enable gravity
        rb.drag = drag; // smoother, slower movement
        rb.angularDrag = drag; // natural spin

        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetBallPosition();
    }

    // for playtesting
    public void ResetBallPosition()
    {
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // when ball is hit by player or object
    public void ApplyHitForce(Vector3 direction, float hitStrength)
    {
        Debug.Log("Ball hit with stick");
        Debug.Log("Applying force: " + direction * hitStrength);
        rb.velocity = Vector3.zero; // reset velocity before applying force
        rb.angularVelocity = Vector3.zero; // prevent unwanted spin

        rb.AddForce(direction * hitStrength, ForceMode.Impulse);
    }
}