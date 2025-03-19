using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject startScreen; // reference to the UI Panel (start screen)
    public GameObject gameObjects; // reference to all your game objects to enable after start

    public Rigidbody ballrb;
    public float initialThrowForce = 5f;

    private bool gameStarted = false;

    void Start()
    {
        startScreen.SetActive(true); // start screen is active initially
        gameObjects.SetActive(false); // game objects are inactive at first
    }

    void Update()
    {
        // detect when ENTER key is pressed
        if (!gameStarted && Input.GetKeyDown(KeyCode.Return))
        {
            StartVolleyBall();
        }
    }

    void StartVolleyBall()
    {
        // disable start screen and enable game objects
        startScreen.SetActive(false);
        gameObjects.SetActive(true);

        // throw ball upwards
        ballrb.velocity = Vector3.zero; // reset velocity
        ballrb.AddForce(Vector3.up * initialThrowForce, ForceMode.Impulse); // apply upward force

        gameStarted = true;
    }
}