using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVRMovement : MonoBehaviour
{
    [SerializeField] private Transform CameraRig;
    [SerializeField] private Transform Head;
    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        //Store current 
        Vector3 oldPosition = CameraRig.position;
        Quaternion oldRotation = CameraRig.rotation;

        //Rotate
        transform.eulerAngles = new Vector3(0, Head.rotation.eulerAngles.y, 0);
        transform.position = new Vector3(Head.position.x, 0, Head.position.z);

        //Restore position
        CameraRig.position = oldPosition;
        CameraRig.rotation = oldRotation;
    }
}
