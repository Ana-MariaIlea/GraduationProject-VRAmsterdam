using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVRMovement : MonoBehaviour
{
    //[SerializeField] private CapsuleCollider characterController = null;
    [SerializeField] private Transform CameraRig;
    [SerializeField] private Transform Head;
    private void Update()
    {
        HandleRotation();
        //HandleMovement();
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

    //private void CalculateMovement()
    //{
    //    // Figure out orientation
    //    Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
    //    Quaternion orientation = Quaternion.Euler(orientationEuler);
    //    Vector3 movement = Vector3.zero;

    //    Vector3 dir = Head.transform.forward;
    //    Vector3 XZPlane = new Vector3(dir.x, 0, dir.z);
    //    XZPlane.Normalize();

    //    // If not moving
    //    if (movePress.GetStateUp(SteamVR_Input_Sources.Any))
    //    {

    //        speed = 0;
    //    }

    //    Debug.DrawRay(Head.position, orientation * (speed * Vector3.forward) * 15, Color.red, 10);

    //    // Debug.DrawRay(transform.position, orientation * (speed * Vector3.forward)*15);

    //    // If buttonpressed

    //    if (movePress.GetState(SteamVR_Input_Sources.Any))
    //    {
    //        speed = MaxSpeed;
    //        if (timer >= footStepsComponent.timeBetweenSteps)
    //        {
    //            GetComponent<FootSteps>().MakeStep();
    //            timer = 0;
    //        }
    //        movement = orientation * (speed * Vector3.forward) * Time.deltaTime;
    //        // movement = XZPlane * speed * Time.deltaTime;
    //        //GetComponent<Rigidbody>().velocity = movement;
    //        transform.position += movement;
    //        // transform.Translate(movement);
    //        // Debug.Log("MOVE");
    //    }
    //    //if (movePress.GetState(SteamVR_Input_Sources.RightHand))
    //    //{
    //    //    speed = 4;
    //    //    movement = orientation * (speed * Vector3.forward) * Time.deltaTime;
    //    //}

    //    //if (movePress.GetState(SteamVR_Input_Sources.LeftHand))
    //    //{
    //    //    speed = -4;
    //    //    movement = orientation * (speed * Vector3.forward) * Time.deltaTime;
    //    //}

    //    // Apply
    //    // characterController.Move(movement);
    //}

    //private void HandleMovement()
    //{
    //    // Get head in local space
    //    float headHeight = Mathf.Clamp(Head.localPosition.y, 1, 2);
    //    characterController.height = headHeight;

    //    // Cut in half
    //    Vector3 newCenter = Vector3.zero;
    //    newCenter.y = characterController.height / 2;
    //    // newCenter.y += characterController.skinWidth;

    //    // Move capsule in local space
    //    newCenter.x = -Head.localPosition.x;
    //    // newCenter.y = Head.localPosition.y;
    //    newCenter.z = -Head.localPosition.z;

    //    // Rotate
    //    newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

    //    // Apply
    //    characterController.center = newCenter;
    //}
}
