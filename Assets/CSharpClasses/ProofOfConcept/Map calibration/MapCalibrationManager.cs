using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages UI and functionality for map calibration.
/// The map calibration happens through adjusting the player's camera transform (OVRCameraRig) happening inside the PlayerCameraCalibration.
/// </summary>

[RequireComponent(typeof(PlayerCameraCalibration))]
public class MapCalibrationManager : MonoBehaviour
{
    public bool shownUiOnStart = true;
    public bool disableLaserPointerOnCalibrationFinish = false;

    [Space(10)]
    [Tooltip("This obj enables interaction with regular UI elements, such as buttons in the calibration steps, through a laser pointer.")]
    public GameObject uiHelpersObj;
    public GameObject uiOriginIndicator;

    [Space(10)]
    [Tooltip("This list represents the consequent steps in the map calibration.")]
    [SerializeField] public CalibrationStep[] calibrationSteps;

    [System.Serializable]
    public struct CalibrationStep
    {
        public GameObject ui;
        public PlayerCameraCalibration.CalibrationControlls assignedControll;
    }
    private PlayerCameraCalibration _playerCamCalib;
    private int _currentUI = -1;// Start with 1st step in the list

    private void Start()
    {
        _playerCamCalib = GetComponent<PlayerCameraCalibration>();
        _playerCamCalib.LoadExistingCalibration();

        uiOriginIndicator.SetActive(true);
        HideAllSteps();
        if (shownUiOnStart)
        {
            ShowNextStep();
        } 
    }
    public void FinishCalibration()
    {
        _playerCamCalib.SaveCurrentCalibration();
        HideAllSteps();
        uiOriginIndicator.SetActive(false);

        if (disableLaserPointerOnCalibrationFinish)
        {
            uiHelpersObj.SetActive(false);
        } 
        
        // Do something before disabling itself.
        // (Perhaps trigger an event?)
        
        this.gameObject.SetActive(false);
    }
    public void RestartCalibration()
    {
        HideAllSteps();

        _currentUI = 0;
        ShowNextStep();
    }
    public void ShowNextStep()
    {
        if (calibrationSteps.Length == 0)
        {
            Debug.LogError($"There are no Calibration Steps assigned to start the map calibration tutorial!");
            return;
        }

        if (_currentUI >= -1)
        {
            if(_currentUI >= 0)
                calibrationSteps[_currentUI].ui.SetActive(false);//hide existing previous

            if(_currentUI < (calibrationSteps.Length - 1))
            {
                _currentUI += 1;
                calibrationSteps[_currentUI].ui.SetActive(true);
                SwitchCalibrationControlls(_currentUI);
            }
            else
            {
                FinishCalibration();
            }
        }
    }
    
    public void ShowPreviousStep()
    {
        if (calibrationSteps.Length == 0)
        {
            Debug.LogError($"There are no Calibration Steps assigned to start the map calibration tutorial!");
            return;
        }

        if (_currentUI > 0)
        {
            calibrationSteps[_currentUI].ui.SetActive(false);//hide existing previous

            if (_currentUI <= (calibrationSteps.Length - 1))
            {
                _currentUI -= 1;
                calibrationSteps[_currentUI].ui.SetActive(true);
                SwitchCalibrationControlls(_currentUI);
            }
        }
    }

    private void HideAllSteps()
    {
        for (int i = 0; i < calibrationSteps.Length; i++)
        {
            calibrationSteps[i].ui.SetActive(false);
        }
    }
    private void SwitchCalibrationControlls(int currentUI)
    {
        _playerCamCalib.calibrationControlls = calibrationSteps[currentUI].assignedControll;
    }
}
