using UnityEngine;

// just for debug, finding dimensions of the court so that the ball wont go outside of the bounds
public class CourtDimensions : MonoBehaviour
{
    void Start()
    {
        // get the collider component attached to the court
        Collider courtCollider = GetComponent<Collider>();

        // get the bounds of the collider (this gives the size of the collider's box)
        Vector3 courtSize = courtCollider.bounds.size;

        // output the dimensions
        Debug.Log("Court Dimensions: " + courtSize);
    }
}