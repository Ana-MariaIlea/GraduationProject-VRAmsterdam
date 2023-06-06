using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

//Quest controllers input in Unity:
//https://docs.unity3d.com/560/Documentation/Manual/OculusControllers.html


/// <summary>
/// This class allows for calibration of the player camera (OVRCameraRig) via controller input (OVRInput).
/// </summary>
public class PlayerCameraCalibration : NetworkBehaviour
{
    [Tooltip("OVRCameraRig transform")]
    public Transform OVRCameraRig;
    [Tooltip("Left hand controller anchor.")]
    public Transform LeftHandController;
    [Space(10)]
    public GameObject canvas;
    [Space(10)]
    public Slider calibrationSpeedSlider;
    [Space(10)]
    public float calibrationSpeedModifier = 0.1f;
    private float _rotSpeed = 0.1f;
    private float _posVerticalSpeed = 0.1f;
    private float _posHorizontalSpeed = 0.01f;
    private float _floorLevelOffset = 0.05f;
    private float _calibrationSpeedMultiplayer = 1;
    private const float MAX_CALIBRATION_SPEED = 2;
    
    private string _posKey = "camPos";
    private string _rotKey = "camRot";
        
    public enum CalibrationControlls
    {
        NONE,
        FLOOR,
        MOVEROTATE,
        ALL
    }
    [Space(10)]
    [Tooltip("Calibration controlls setting applied at the Start of the application until changed later.")]
    public CalibrationControlls calibrationControlls = CalibrationControlls.NONE;

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            if (OVRCameraRig == null)
            {
                Debug.LogError($"Camera calibration is not possible. CameraRig reference is not defined!");
                if (!LeftHandController)
                    Debug.LogError($"Floor calibration is not possible. LeftHandController reference is not defined!");
            }
            else
            {
                canvas.SetActive(true);
                InitiateCalibrationSpeedSlider();
            }
            base.OnNetworkSpawn();
        }
    }


    void Update()
    {
        if (IsClient && IsOwner)
        {
            switch (calibrationControlls)
            {
                case CalibrationControlls.FLOOR:
                    CalibrateFloorLevel();
                    break;
                case CalibrationControlls.MOVEROTATE:
                    RotateCameraAround();
                    MoveCameraLeftRight();
                    MoveCameraForwardBackward();
                    break;
                case CalibrationControlls.ALL:
                    RotateCameraAround();
                    MoveCameraLeftRight();
                    MoveCameraForwardBackward();
                    CalibrateFloorLevel();
                    SaveCurrentCalibration();
                    LoadExistingCalibration();
                    break;
                default:
                    break;
            }
        }
    }

    private void RotateCameraAround()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;

        if (axisValue != 0)
        {
            //Left
            if (axisValue < 0)
                OVRCameraRig.Rotate(Vector3.up, (_rotSpeed * _calibrationSpeedMultiplayer), Space.Self);

            //Right
            if (axisValue > 0)
                OVRCameraRig.Rotate(Vector3.up, -(_rotSpeed * _calibrationSpeedMultiplayer), Space.Self);
        }
    }
    private void MoveCameraForwardBackward()
    {
        //B - front
        if (OVRInput.Get(OVRInput.Button.Four))
        {
            OVRCameraRig.position = new Vector3(
                    OVRCameraRig.position.x,
                    OVRCameraRig.position.y,
                    OVRCameraRig.position.z - (_posVerticalSpeed * _calibrationSpeedMultiplayer));
        }

        //A - back
        if (OVRInput.Get(OVRInput.Button.Three))
        {
            OVRCameraRig.position = new Vector3(
                    OVRCameraRig.position.x,
                    OVRCameraRig.position.y,
                    OVRCameraRig.position.z + (_posVerticalSpeed * _calibrationSpeedMultiplayer));
        }
    }
    private void MoveCameraLeftRight()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

        if (axisValue != 0)
        {
            //Forward
            if (axisValue < 0)
                OVRCameraRig.position = new Vector3(
                    OVRCameraRig.position.x + (_posHorizontalSpeed * _calibrationSpeedMultiplayer),
                    OVRCameraRig.position.y,
                    OVRCameraRig.position.z);

            //Backward
            if (axisValue > 0)
                OVRCameraRig.position = new Vector3(
                    OVRCameraRig.position.x - (_posHorizontalSpeed * _calibrationSpeedMultiplayer),
                    OVRCameraRig.position.y,
                    OVRCameraRig.position.z);
        }
    }
    /// <summary>
    /// Allows for floor level calibration by making the player CameraRig.y higher or lower based on the controller.y position when the left grab button is pressed.
    /// (Left grab button = OVRInput.Axis1D.PrimaryHandTrigger)
    /// </summary>
    private void CalibrateFloorLevel()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        if (axisValue == 1.0f)
        {
            float controllerY = LeftHandController.position.y;
            if (controllerY < 0)
            {
                //Physical floor under virtual -> Make player higher
                OVRCameraRig.position = new Vector3(OVRCameraRig.position.x, OVRCameraRig.position.y + (controllerY * -1) + _floorLevelOffset, OVRCameraRig.position.z);
            }
            else if (controllerY > 0)
            {
                //Physical floor above virtual -> Make player smaller
                OVRCameraRig.position = new Vector3(OVRCameraRig.position.x, OVRCameraRig.position.y - controllerY + _floorLevelOffset, OVRCameraRig.position.z);
            }
        }
    }


    /// <summary>
    /// Saves current position and rotation of the CameraRig into PlayerPreferences (local memory available only to this application).
    /// </summary>
    public void SaveCurrentCalibration()
    {
        PlayerPrefs.SetFloat(_posKey + "x", OVRCameraRig.position.x);
        PlayerPrefs.SetFloat(_posKey + "y", OVRCameraRig.position.y);
        PlayerPrefs.SetFloat(_posKey + "z", OVRCameraRig.position.z);

        PlayerPrefs.SetFloat(_rotKey + "x", OVRCameraRig.rotation.x);
        PlayerPrefs.SetFloat(_rotKey + "y", OVRCameraRig.rotation.y);
        PlayerPrefs.SetFloat(_rotKey + "z", OVRCameraRig.rotation.z);
        PlayerPrefs.SetFloat(_rotKey + "w", OVRCameraRig.rotation.w);
    }
    /// <summary>
    /// Loads any existing camera calibration data from the PlayerPreferences.
    /// </summary>
    public void LoadExistingCalibration()
    {
        //if (OVRCameraRig.gameObject.activeSelf)
        //{
            Vector3 loadedPos = Vector3.zero;
            loadedPos.x = PlayerPrefs.GetFloat(_posKey + "x");
            loadedPos.y = PlayerPrefs.GetFloat(_posKey + "y");
            loadedPos.z = PlayerPrefs.GetFloat(_posKey + "z");

            Quaternion loadedRot = Quaternion.identity;
            loadedRot.x = PlayerPrefs.GetFloat(_rotKey + "x");
            loadedRot.y = PlayerPrefs.GetFloat(_rotKey + "y");
            loadedRot.z = PlayerPrefs.GetFloat(_rotKey + "z");
            loadedRot.w = PlayerPrefs.GetFloat(_rotKey + "w");

            if (loadedPos != Vector3.zero)
                OVRCameraRig.position = loadedPos;
            if (loadedRot != Quaternion.identity)
                OVRCameraRig.rotation = loadedRot;
        //}
    }

    /// <summary>
    /// Map calibration UI button method
    /// </summary>
    public void IncreaseCalibrationSpeed()
    {
        if(_calibrationSpeedMultiplayer <= (MAX_CALIBRATION_SPEED - calibrationSpeedModifier))
        {
            _calibrationSpeedMultiplayer += calibrationSpeedModifier;
            calibrationSpeedSlider.value = _calibrationSpeedMultiplayer;
        }        
    }
    /// <summary>
    /// Map calibration UI button method
    /// </summary>
    public void DecreaseCalibrationSpeed()
    {
        if (_calibrationSpeedMultiplayer >= calibrationSpeedModifier)
        {
            _calibrationSpeedMultiplayer -= calibrationSpeedModifier;
            calibrationSpeedSlider.value = _calibrationSpeedMultiplayer;
        }
    }

    private void InitiateCalibrationSpeedSlider()
    {
        if(calibrationSpeedSlider != null)
        {
            calibrationSpeedSlider.maxValue = MAX_CALIBRATION_SPEED;
            calibrationSpeedSlider.value = MAX_CALIBRATION_SPEED/2;
        }
        
    }
}