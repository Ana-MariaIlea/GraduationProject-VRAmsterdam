using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.Composites;

//Quest controllers input in Unity:
//https://docs.unity3d.com/560/Documentation/Manual/OculusControllers.html


/// <summary>
/// This class allows for calibration of the player camera (OVRCameraRig) via controller input (OVRInput).
/// </summary>
public class PlayerCameraCalibration : NetworkBehaviour
{
    public Transform CameraRig;
    public Transform LeftHandController;

    private float rotSpeed = 0.1f;
    private float posVerticalSpeed = 0.01f;
    private float posHorizontalSpeed = 0.001f;
    private float floorLevelOffset = 0.05f;

    private string posKey = "camPos";
    private string rotKey = "camRot";

    public DebugPanel debugTextPanel;

    private void Start()
    {
        if (CameraRig == null)
        {
            if (!debugTextPanel)
            {
                debugTextPanel.PrintDebug("Camera calibration is not possible. CameraRig reference is not defined!", DebugPanel.Colors.RED);
                Debug.LogError($"Camera calibration is not possible. CameraRig reference is not defined!");
            }
            if (!debugTextPanel)
            {
                if (!LeftHandController)
                {
                    debugTextPanel.PrintDebug("Floor calibration is not possible. LeftHandController reference is not defined!", DebugPanel.Colors.RED);
                    Debug.LogError($"Floor calibration is not possible. LeftHandController reference is not defined!");
                }
            }
        }
    }


    void Update()
    {
        if (IsClient && IsOwner)
        {
            RotateCameraAround();
            MoveCameraLeftRight();
            MoveCameraForwardBackward();
            CalibrateFloorLevel();

            SaveCurrentCalibration();
            LoadExistingCalibration();
        }
    }

    private void RotateCameraAround()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;

        if (axisValue != 0)
        {
            //Left
            if (axisValue < 0)
                CameraRig.Rotate(Vector3.up, rotSpeed, Space.Self);

            //Right
            if (axisValue > 0)
                CameraRig.Rotate(Vector3.up, -rotSpeed, Space.Self);
        }
    }
    private void MoveCameraForwardBackward()
    {
        //B - front
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            CameraRig.position = new Vector3(
                    CameraRig.position.x,
                    CameraRig.position.y,
                    CameraRig.position.z - posVerticalSpeed);
        }

        //A - back
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            CameraRig.position = new Vector3(
                    CameraRig.position.x,
                    CameraRig.position.y,
                    CameraRig.position.z + posVerticalSpeed);
        }
    }
    private void MoveCameraLeftRight()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

        if (axisValue != 0)
        {
            //Forward
            if (axisValue < 0)
                CameraRig.position = new Vector3(
                    CameraRig.position.x + posHorizontalSpeed,
                    CameraRig.position.y,
                    CameraRig.position.z);

            //Backward
            if (axisValue > 0)
                CameraRig.position = new Vector3(
                    CameraRig.position.x - posHorizontalSpeed,
                    CameraRig.position.y,
                    CameraRig.position.z);
        }
    }

    private void CalibrateFloorLevel()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        if (axisValue == 1.0f)
        {
            float controllerY = LeftHandController.position.y;
            if (controllerY < 0)
            {
                //Physical floor under virtual -> Make player higher
                CameraRig.position = new Vector3(CameraRig.position.x, CameraRig.position.y + (controllerY * -1) + floorLevelOffset, CameraRig.position.z);
            }
            else if (controllerY > 0)
            {
                //Physical floor above virtual -> Make player smaller
                CameraRig.position = new Vector3(CameraRig.position.x, CameraRig.position.y - controllerY + floorLevelOffset, CameraRig.position.z);
            }
        }
    }


    /// <summary>
    /// Saves current position and rotation of the CameraRig into PlayerPreferences (local memory available only to this application).
    /// </summary>
    private void SaveCurrentCalibration()
    {
        //X on Left controller
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            PlayerPrefs.SetFloat(posKey + "x", CameraRig.position.x);
            PlayerPrefs.SetFloat(posKey + "y", CameraRig.position.y);
            PlayerPrefs.SetFloat(posKey + "z", CameraRig.position.z);

            PlayerPrefs.SetFloat(rotKey + "x", CameraRig.rotation.x);
            PlayerPrefs.SetFloat(rotKey + "y", CameraRig.rotation.y);
            PlayerPrefs.SetFloat(rotKey + "z", CameraRig.rotation.z);
            PlayerPrefs.SetFloat(rotKey + "w", CameraRig.rotation.w);
        }
    }
    /// <summary>
    /// Loads any existing camera calibration data from the PlayerPreferences.
    /// </summary>
    public void LoadExistingCalibration()
    {
        //Y on Left controller
        if (OVRInput.GetDown(OVRInput.Button.Four))
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
                CameraRig.position = loadedPos;
            if (loadedRot != Quaternion.identity)
                CameraRig.rotation = loadedRot;
        }
    }
}