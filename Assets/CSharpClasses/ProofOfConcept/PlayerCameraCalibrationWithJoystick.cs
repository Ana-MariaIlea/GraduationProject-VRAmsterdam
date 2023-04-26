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

    private string posKey = "camPos";
    private string rotKey = "camRot";

    void Update()
    {
        if (IsClient && IsOwner)
        {
            float joystickLeftHoriz = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
            float joystickRightHoriz = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

            RotateCamera(joystickLeftHoriz);
            MoveCameraLeftRight(joystickRightHoriz);
            MoveCameraForwardBack();

            //X on Left controller
            if (OVRInput.GetDown(OVRInput.Button.Three))
                SaveCurrentCameraCalibration();
            //Y on Left controller
            if (OVRInput.GetDown(OVRInput.Button.Four))
                LoadExistingCameraCalibration();
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


    /// <summary>
    /// Saves current position and rotation of the CameraRig into PlayerPreferences (local memory available only to this application).
    /// </summary>
    private void SaveCurrentCameraCalibration()
    {
        PlayerPrefs.SetFloat(posKey + "x", oculusCameraRigTransform.position.x);
        PlayerPrefs.SetFloat(posKey + "y", oculusCameraRigTransform.position.y);
        PlayerPrefs.SetFloat(posKey + "z", oculusCameraRigTransform.position.z);

        PlayerPrefs.SetFloat(rotKey + "x", oculusCameraRigTransform.rotation.x);
        PlayerPrefs.SetFloat(rotKey + "y", oculusCameraRigTransform.rotation.y);
        PlayerPrefs.SetFloat(rotKey + "z", oculusCameraRigTransform.rotation.z);
        PlayerPrefs.SetFloat(rotKey + "w", oculusCameraRigTransform.rotation.w);
    }
    /// <summary>
    /// Loads any existing camera calibration data from the PlayerPreferences.
    /// </summary>
    public void LoadExistingCameraCalibration()
    {
        Vector3 loadedPos = Vector3.zero;
        loadedPos.x = PlayerPrefs.GetFloat(posKey + "x");
        loadedPos.y = PlayerPrefs.GetFloat(posKey + "y");
        loadedPos.z = PlayerPrefs.GetFloat(posKey + "z");

        Quaternion loadedRot = Quaternion.identity;
        loadedRot.x = PlayerPrefs.GetFloat(rotKey + "x");
        loadedRot.y = PlayerPrefs.GetFloat(rotKey + "y");
        loadedRot.z = PlayerPrefs.GetFloat(rotKey + "z");
        loadedRot.w = PlayerPrefs.GetFloat(rotKey + "w");

        if (loadedPos != Vector3.zero)
            oculusCameraRigTransform.position = loadedPos;
        if (loadedRot != Quaternion.identity)
            oculusCameraRigTransform.rotation = loadedRot;
    }
}