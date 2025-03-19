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
    public Rigidbody rb; // accessed by stickSwing to determine hitDirection
    private Vector3 initialPosition;

    // audio
    public AudioSource audioSource;
    public AudioClip bounceSound;
    public AudioClip whistleSound;

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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball collided with: " + collision.gameObject.name);
        if (collision.gameObject.name == "sand" || collision.gameObject.name == "FLOOR" || collision.gameObject.name == "NET")
        {
            // play bounce sound effect
            audioSource.PlayOneShot(bounceSound);

            // play whistle noise after a delay of 0.5 seconds
            Invoke("PlayWhistle", 0.5f);
        }
    }

    void PlayWhistle()
    {
        audioSource.PlayOneShot(whistleSound, 0.3f); // 30% volume
    }
}