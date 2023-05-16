using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
