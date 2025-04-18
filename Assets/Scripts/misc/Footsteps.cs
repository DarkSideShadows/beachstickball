using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public GameObject footstep;
    public PlayerController playerController;

    void Start()
    {
        footstep.SetActive(false);
    }

    void Update()
    {
        // get input type: 1 (WASD) or 2 (arrow keys)
        float horizontal = Input.GetAxis(playerController.horizontalInput);
        float vertical = Input.GetAxis(playerController.verticalInput);

        // check speed
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            footsteps();
        else 
            StopFootsteps();
    }

    void footsteps()
    {
        footstep.SetActive(true);
    }

    void StopFootsteps()
    {
        footstep.SetActive(false);
    }
}