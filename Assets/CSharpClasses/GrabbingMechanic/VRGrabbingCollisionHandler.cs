//Made by Ana-Maria Ilea

using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Handles trigger enter and stay between the controllers and the items on the client side
//      Script is used due to the server not having access to the controllers
// </summary>
//------------------------------------------------------------------------------
public class VRGrabbingCollisionHandler : MonoBehaviour
{
    [SerializeField] private PlayerVRGrabbing grabbing;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabbing.TriggerEnterGrab(other);
        }
        else if (other.tag == "GrabDestination")
        {
            grabbing.TriggerEnterGrabDestination(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        grabbing.TriggerExit();
    }
}
