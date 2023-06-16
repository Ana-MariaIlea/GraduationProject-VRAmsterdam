using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages the switching of UI steps in the map calibration process.
/// Based on the current step it switches the controllers executed in the PlayerCameraCalibration script.
/// 
/// Author: Kristyna Pavlatova
/// Date: June 2023
/// </summary>

[RequireComponent(typeof(PlayerCameraCalibration))]
public class MapCalibrationUiManager : MonoBehaviour
{
    public bool shownUiOnStart = true;
    public bool disableLaserPointerOnCalibrationFinish = false;

    [Space(10)]
    [Tooltip("This obj enables interaction with regular UI elements, such as buttons in the calibration steps, through a laser pointer.")]
    public GameObject uiHelpersObj;
    public GameObject originIndicator;

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
        HideAllSteps();

        if (shownUiOnStart)
        {
            StartMapCalibration();
        }
    }
    public void StartMapCalibration()
    {
        originIndicator.SetActive(true);
        ShowNextStep();
    }
    public void FinishCalibration()
    {
        _playerCamCalib.SaveCurrentCalibration();
        HideAllSteps();
        originIndicator.SetActive(false);

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
