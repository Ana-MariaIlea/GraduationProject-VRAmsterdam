using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCameraCalibrationWithJoystick : NetworkBehaviour
{
    public Transform oculusCameraRigTransform;

    private float rotSpeed = 0.1f;
    private float posVerticalSpeed = 0.01f;
    private float posHorizontalSpeed = 0.001f;



    void Update()
    {
        if (IsClient && IsOwner)
        {
            float joystickLeftHoriz = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
            float joystickRightHoriz = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

            RotateCamera(joystickLeftHoriz);
            MoveCameraLeftRight(joystickRightHoriz);
            MoveCameraForwardBack();
        }
    }

    private void RotateCamera(float axisValue)
    {
        if (axisValue != 0)
        {
            //Left
            if (axisValue < 0)
                oculusCameraRigTransform.Rotate(Vector3.up, rotSpeed, Space.Self);

            //Right
            if (axisValue > 0)
                oculusCameraRigTransform.Rotate(Vector3.up, -rotSpeed, Space.Self);
        }
    }
    private void MoveCameraForwardBack()
    {
        //B - front
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            oculusCameraRigTransform.position = new Vector3(
                    oculusCameraRigTransform.position.x,
                    oculusCameraRigTransform.position.y,
                    oculusCameraRigTransform.position.z - posVerticalSpeed);
        }
        
        //A - back
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            oculusCameraRigTransform.position = new Vector3(
                    oculusCameraRigTransform.position.x,
                    oculusCameraRigTransform.position.y,
                    oculusCameraRigTransform.position.z + posVerticalSpeed);
        }
    }
    private void MoveCameraLeftRight(float axisValue)
    {
        if (axisValue != 0)
        {
            //Forward
            if (axisValue < 0)
                oculusCameraRigTransform.position = new Vector3(
                    oculusCameraRigTransform.position.x + posHorizontalSpeed,
                    oculusCameraRigTransform.position.y,
                    oculusCameraRigTransform.position.z);

            //Backward
            if (axisValue > 0)
                oculusCameraRigTransform.position = new Vector3(
                    oculusCameraRigTransform.position.x - posHorizontalSpeed,
                    oculusCameraRigTransform.position.y,
                    oculusCameraRigTransform.position.z);
        }
    }
}
