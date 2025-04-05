using Unity.Netcode.Components;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

/*
    this script determines the gameOver flag
    - disables input for both players
    - asks to play again, displays score

    only the server should control the ball
    - ball is synced across clients
    - reset + force application happens server-side
 */
public class BallController : NetworkBehaviour
{
    public Rigidbody rb;
    private Vector3 initialPosition;

    // game over logic
    public SpeechBubbleController speechBubbleController; // display score, ask to play again
    public int hitCount = 0;
    private bool gameOver = false;
    private bool first = true;

    // tunables
    public float drag = 0.5f;
    public float initialThrowForce = 5f;

    // audio
    public AudioSource audioSource;
    public AudioClip bounceSound;
    public AudioClip whistleSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // freeze ball
        rb.useGravity = false; // freeze ball

        // set spawn position and note for reset
        initialPosition = transform.position;
    }

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
            ResetBallPosition();
    }

    // resetting ball after failed attempt
    public void ResetBallPosition()
    {
        // reset game state
        hitCount = 0;
        gameOver = false;

        // server
        ActivateBallServerRpc();     // resets ball position and physics
        HideSpeechBubbleClientRpc(); // hides speech bubble on all clients
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateBallServerRpc(ServerRpcParams rpcParams = default) // spawn ball when we start game
    {
        if (first)
        {
            ActivateBallClientRpc(); // tell clients to activate volleyball visuals
            first = false;
        }

        // 1. stop physics before resetting
        rb.isKinematic = true;
        rb.useGravity = false;

        // 2. teleport the ball
        GetComponent<NetworkTransform>().Teleport(initialPosition, transform.rotation, transform.localScale);

        // 3. wait one physics frame before reactivating
        StartCoroutine(ResumeAfterReset());
    }

    IEnumerator ResumeAfterReset()
    {
        yield return new WaitForFixedUpdate();

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(Vector3.up * initialThrowForce, ForceMode.Impulse);
    }

    [ClientRpc]
    void ActivateBallClientRpc()
    {
        GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    [ServerRpc(RequireOwnership = false)] // ownership = false, client can hit the ball
    public void ApplyHitForceServerRpc(Vector3 direction, float hitStrength) // when ball is hit by player or object
    {
        rb.linearVelocity = Vector3.zero;   // reset velocity before applying force
        rb.angularVelocity = Vector3.zero;  // prevent unwanted spin
        rb.AddForce(direction * hitStrength, ForceMode.Impulse);
        hitCount++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball collided with: " + collision.gameObject.name);
        // collision with anything but stick results in game loss
        if (collision.gameObject.name == "sand" || collision.gameObject.name == "FLOOR" || collision.gameObject.name == "NET")
        {
            // play bounce sound effect
            audioSource.PlayOneShot(bounceSound);

            // play whistle noise after a delay of 0.5 seconds
            Invoke("PlayWhistle", 0.5f);

            // display result
            ShowSpeechBubbleClientRpc($"You got {hitCount} hits, press R to reset!");

            // wait for player to reset
            gameOver = true;
        }
    }

    [ClientRpc]
    void ShowSpeechBubbleClientRpc(string message)
    {
        speechBubbleController.ShowSpeechBubble(message);
    }

    [ClientRpc]
    void HideSpeechBubbleClientRpc()
    {
        speechBubbleController.HideSpeechBubble();
    }

    void PlayWhistle()
    {
        audioSource.PlayOneShot(whistleSound, 0.3f); // 30% volume
    }
}