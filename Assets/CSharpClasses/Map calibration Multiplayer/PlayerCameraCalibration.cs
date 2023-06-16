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
    public GameObject canvas;

    [Space(10)]
    [Header("Player Camera:")]
    [Tooltip("OVRCameraRig transform")]
    public Transform ovrCameraRig;
    [Tooltip("Transform that will give the Y position of the floor level on controller button pressed.")]
    public Transform floorLevelTransform;

    [Space(10)]
    [Header("Calibration Speed:")]
    [Tooltip("Value to modify the current calibrationSpeedMultiplayer value by. Basically the sensitivity of the slider.")]
    public float calibrationSpeedModifier = 0.1f;
    public float rotationSpeed = 0.1f;
    public float frowardBackwardsSpeed = 0.01f;
    public float rightLeftSpeed = 0.01f;
    private float THUMBSTICK_SENSITIVITY_THRESHOLD = 0.5f;
    [Tooltip("The floor's resulting Y position has an offset from the concrete Y position of the controller.")]
    public float floorLevelOffset = 0.05f;
    private float _calibrationSpeedMultiplayer = 1;// The initial value all calibration speed values will be multiplayed by. Gets changed by the calibrationSpeedModifier
    private const float MAX_CALIBRATION_SPEED = 2;
    public Slider calibrationSpeedUiSlider;

    [Space(10)]
    [Header("Floor calibration:")]
    [Tooltip("Reference to the animator component that holds animation for the floor calibration progress circle.")]
    public Animator floorCalibrationAnimator;
    private bool _isFloorCalibrationInProgress = false;
    private float CUSTOM_ANIMATION_LENGTH = 1.0f;

    private string _savedPositionKey = "cameraPosition";
    private string _savedRotationKey = "cameraRotation";

    public enum CalibrationControlls
    {
        NONE,
        FLOOR,
        MOVEROTATE,
        ALL
    }
    [HideInInspector] public CalibrationControlls calibrationControlls = CalibrationControlls.NONE;

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            if (ovrCameraRig == null)
            {
                Debug.LogError($"Camera calibration is not possible. CameraRig reference is not defined!");
                if (!floorLevelTransform)
                    Debug.LogError($"Floor calibration is not possible. LeftHandController reference is not defined!");
            }
            else
            {
                canvas.SetActive(true);
                InitiateValuesInCalibrationSpeedSlider();
                ShowAndPlayFloorCalibrationAnimation(false);
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

        if (axisValue >= THUMBSTICK_SENSITIVITY_THRESHOLD || axisValue <= (THUMBSTICK_SENSITIVITY_THRESHOLD * -1))
        {
            //Left
            if (axisValue < 0)
            {
                ovrCameraRig.Rotate(Vector3.up, (rotationSpeed * _calibrationSpeedMultiplayer), Space.Self);
            }
            //Right
            if (axisValue > 0)
            {
                ovrCameraRig.Rotate(Vector3.up, -(rotationSpeed * _calibrationSpeedMultiplayer), Space.Self);
            }
        }
    }
    private void MoveCameraForwardBackward()
    {
        //B - front
        if (OVRInput.Get(OVRInput.Button.Four))
        {
            ovrCameraRig.position = new Vector3(
                    ovrCameraRig.position.x,
                    ovrCameraRig.position.y,
                    ovrCameraRig.position.z - (frowardBackwardsSpeed * _calibrationSpeedMultiplayer));
        }

        //A - back
        if (OVRInput.Get(OVRInput.Button.Three))
        {
            ovrCameraRig.position = new Vector3(
                    ovrCameraRig.position.x,
                    ovrCameraRig.position.y,
                    ovrCameraRig.position.z + (frowardBackwardsSpeed * _calibrationSpeedMultiplayer));
        }
    }
    private void MoveCameraLeftRight()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

        if (axisValue >= THUMBSTICK_SENSITIVITY_THRESHOLD || axisValue <= (THUMBSTICK_SENSITIVITY_THRESHOLD * -1))
        {
            //Forward
            if (axisValue < 0)
                ovrCameraRig.position = new Vector3(
                    ovrCameraRig.position.x + (rightLeftSpeed * _calibrationSpeedMultiplayer),
                    ovrCameraRig.position.y,
                    ovrCameraRig.position.z);

            //Backward
            if (axisValue > 0)
                ovrCameraRig.position = new Vector3(
                    ovrCameraRig.position.x - (rightLeftSpeed * _calibrationSpeedMultiplayer),
                    ovrCameraRig.position.y,
                    ovrCameraRig.position.z);
        }
    }
    /// <summary>
    /// Changes the player's height based on the controller.y.
    /// Invokes the "SetFloorLevelToControllerY" function once UI animation is over.
    /// </summary>
    private void CalibrateFloorLevel()
    {
        float axisValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        if (axisValue == 1.0f && !_isFloorCalibrationInProgress)
        {
            _isFloorCalibrationInProgress = true;

            if (floorCalibrationAnimator != null)
            {
                ShowAndPlayFloorCalibrationAnimation(_isFloorCalibrationInProgress);
            }
            else
            {
                Debug.LogError($"Reference to floorCalibrationAnimator is missing!");
            }
            Invoke("SetFloorLevelToControllerY", CUSTOM_ANIMATION_LENGTH);// feels more responsive than the precise value from floorCalibrationAnimator.GetCurrentAnimatorStateInfo(0).length
        }
    }
    private void SetFloorLevelToControllerY()
    {
        float controllerY = floorLevelTransform.position.y;
        if (controllerY < 0)
        {
            // Physical floor under virtual -> Make player higher
            ovrCameraRig.position = new Vector3(ovrCameraRig.position.x, ovrCameraRig.position.y + (controllerY * -1) + floorLevelOffset, ovrCameraRig.position.z);
        }
        else if (controllerY > 0)
        {
            // Physical floor above virtual -> Make player smaller
            ovrCameraRig.position = new Vector3(ovrCameraRig.position.x, ovrCameraRig.position.y - controllerY + floorLevelOffset, ovrCameraRig.position.z);
        }

        _isFloorCalibrationInProgress = false;
        ShowAndPlayFloorCalibrationAnimation(_isFloorCalibrationInProgress);
    }

    public void SaveCurrentCalibration()
    {
        PlayerPrefs.SetFloat(_savedPositionKey + "x", ovrCameraRig.position.x);
        PlayerPrefs.SetFloat(_savedPositionKey + "y", ovrCameraRig.position.y);
        PlayerPrefs.SetFloat(_savedPositionKey + "z", ovrCameraRig.position.z);

        PlayerPrefs.SetFloat(_savedRotationKey + "x", ovrCameraRig.rotation.x);
        PlayerPrefs.SetFloat(_savedRotationKey + "y", ovrCameraRig.rotation.y);
        PlayerPrefs.SetFloat(_savedRotationKey + "z", ovrCameraRig.rotation.z);
        PlayerPrefs.SetFloat(_savedRotationKey + "w", ovrCameraRig.rotation.w);
    }
    public void LoadExistingCalibration()
    {
        Vector3 loadedPos = Vector3.zero;
        loadedPos.x = PlayerPrefs.GetFloat(_savedPositionKey + "x");
        loadedPos.y = PlayerPrefs.GetFloat(_savedPositionKey + "y");
        loadedPos.z = PlayerPrefs.GetFloat(_savedPositionKey + "z");

        Quaternion loadedRot = Quaternion.identity;
        loadedRot.x = PlayerPrefs.GetFloat(_savedRotationKey + "x");
        loadedRot.y = PlayerPrefs.GetFloat(_savedRotationKey + "y");
        loadedRot.z = PlayerPrefs.GetFloat(_savedRotationKey + "z");
        loadedRot.w = PlayerPrefs.GetFloat(_savedRotationKey + "w");

        if (loadedPos != Vector3.zero)
            ovrCameraRig.position = loadedPos;
        if (loadedRot != Quaternion.identity)
            ovrCameraRig.rotation = loadedRot;
    }

    public void IncreaseCalibrationSpeed()
    {
        if (_calibrationSpeedMultiplayer <= (MAX_CALIBRATION_SPEED - calibrationSpeedModifier))
        {
            _calibrationSpeedMultiplayer += calibrationSpeedModifier;
            SetValueToCalibrationSpeedSlider(_calibrationSpeedMultiplayer);
        }
    }
    public void DecreaseCalibrationSpeed()
    {
        if (_calibrationSpeedMultiplayer >= calibrationSpeedModifier)
        {
            _calibrationSpeedMultiplayer -= calibrationSpeedModifier;
            SetValueToCalibrationSpeedSlider(_calibrationSpeedMultiplayer);
        }
    }

    private void InitiateValuesInCalibrationSpeedSlider()
    {
        if (calibrationSpeedUiSlider != null)
        {
            calibrationSpeedUiSlider.maxValue = MAX_CALIBRATION_SPEED;
            SetValueToCalibrationSpeedSlider(MAX_CALIBRATION_SPEED / 2);
        }
    }
    public void SetValueToCalibrationSpeedSlider(float value)
    {
        calibrationSpeedUiSlider.value = value;
    }
    private void ShowAndPlayFloorCalibrationAnimation(bool isVisible)
    {
        floorCalibrationAnimator.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            floorCalibrationAnimator.Play("FloorLevelAnimation");
        }
    }
}