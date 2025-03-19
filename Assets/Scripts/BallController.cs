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
    public float initialThrowForce = 5f;
    public Rigidbody rb; // accessed by stickSwing to determine hitDirection
    private Vector3 initialPosition;

    // counter for number of hits
    public int hitCount = 0;

    // flag to check if game is going on
    private bool gameOver = false;

    public SpeechBubbleController speechBubbleController;
    public PlayerController playerController1;
    public PlayerController playerController2;

    // audio
    public AudioSource audioSource;
    public AudioClip bounceSound;
    public AudioClip whistleSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // re-enable gravity
        rb.linearDamping = drag; // smoother, slower movement
        rb.angularDamping = drag; // natural spin

        initialPosition = rb.position;
    }

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
            ResetBallPosition();
    }

    // for playtesting
    public void ResetBallPosition()
    {
        // reset game state
        hitCount = 0;
        gameOver = false;
        speechBubbleController.HideSpeechBubble();
        playerController1.enabled = true;
        playerController2.enabled = true;

        rb.position = initialPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // throw ball upwards
        rb.AddForce(Vector3.up * initialThrowForce, ForceMode.Impulse); // apply upward force
    }

    // when ball is hit by player or object
    public void ApplyHitForce(Vector3 direction, float hitStrength)
    {
        Debug.Log("Ball hit with stick");
        Debug.Log("Applying force: " + direction * hitStrength);

        rb.linearVelocity = Vector3.zero; // reset velocity before applying force
        rb.angularVelocity = Vector3.zero; // prevent unwanted spin
        rb.AddForce(direction * hitStrength, ForceMode.Impulse);

        hitCount++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Ball collided with: " + collision.gameObject.name);
        // collision with anything but stick results in game loss
        if (collision.gameObject.name == "sand" || collision.gameObject.name == "FLOOR" || collision.gameObject.name == "NET")
        {
            // play bounce sound effect
            audioSource.PlayOneShot(bounceSound);

            // play whistle noise after a delay of 0.5 seconds
            Invoke("PlayWhistle", 0.5f);

            // display result
            speechBubbleController.ShowSpeechBubble($"You got {hitCount} hits, press R to reset!");

            // wait for player to reset
            gameOver = true;
            playerController1.enabled = false; // disable player input
            playerController2.enabled = false; // disable player input
        }
    }

    void PlayWhistle()
    {
        audioSource.PlayOneShot(whistleSound, 0.3f); // 30% volume
    }
}