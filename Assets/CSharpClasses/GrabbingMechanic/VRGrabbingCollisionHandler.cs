using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrabbingCollisionHandler : MonoBehaviour
{
    [SerializeField] private PlayerVRGrabbing grabbing;

    OculusHaptics oculusHaptics;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabbing.TriggerEnterGrab(other);
            //oculusHaptics.Vibrate(OculusHaptics.VibrationForce.Medium);
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
