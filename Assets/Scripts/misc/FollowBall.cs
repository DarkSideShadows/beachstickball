using UnityEngine;

/* make character look at volleyball */
public class FollowBall : MonoBehaviour
{
    public Transform ball;
    public float rotationSpeed = 10f;
    private Quaternion targetRotation;

    void Start()
    {
        FaceBall(); // face the ball at the start
    }

    void Update()
    {
        if (ball != null)
        {
            FaceBall(); // continually face ball during the game
        }
    }

    void FaceBall()
    {
        Vector3 directionToBall = ball.position - transform.position;
        directionToBall.y = 0f; // ignore the vertical direction to keep the rotation on the horizontal plane

        // smoothly follow ball
        targetRotation = Quaternion.LookRotation(directionToBall);
        targetRotation *= Quaternion.Euler(0f, 90f, 0f); // rotate by 90 degrees on the Y-axis (manual adjustment)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
