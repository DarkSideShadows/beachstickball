using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject startScreen; // reference to the UI Panel (start screen)
    public GameObject gameObjects; // reference to all your game objects to enable after start


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

    public void StartVolleyBall()
    {
        // disable start screen and enable game objects
        startScreen.SetActive(false);
        gameObjects.SetActive(true);

        gameStarted = true;
    }
}