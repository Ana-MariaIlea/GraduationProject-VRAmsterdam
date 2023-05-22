using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages UI and functionality for map calibration before the start of the game.
/// The map calibration happens through adjusting the player's camera transform (OVRCameraRig).
/// </summary>

[RequireComponent(typeof(PlayerCameraCalibration))]
public class MapCalibrationManager : MonoBehaviour
{
    public bool showOnStart = true;
    [Space(10)]
    public List<GameObject> CalibrationUis;
    [Space(10)]
    public GameObject uiHelperObj;


    private PlayerCameraCalibration _playerCamCalib;
    private int _currentUI = -1;

    private void Start()
    {
        _playerCamCalib = GetComponent<PlayerCameraCalibration>();

        _playerCamCalib.LoadExistingCalibration();

        hideAllCalibrationUIs();
        if (showOnStart)
            ShowNextUI();


        PlayerStateManager.Singleton.part1StartClient.AddListener(proceedToPart1);
    }
    public void ShowNextUI()
    {
        if (CalibrationUis.Count == 0)
        {
            Debug.LogError($"There are no Calibration UIs assigned to start the map calibration tutorial!");
            return;
        }

        if (_currentUI >= -1)
        {
            if(_currentUI >= 0)
                CalibrationUis[_currentUI].SetActive(false);//hide existing previous

            if(_currentUI < (CalibrationUis.Count - 1))
            {
                _currentUI += 1;
                CalibrationUis[_currentUI].SetActive(true);
                switchCameraCalibrationControlls(_currentUI);
            }
            else calibrationFinished();
        }
    }
    private void hideAllCalibrationUIs()
    {
        foreach(GameObject ui in CalibrationUis)
        {
            ui.SetActive(false);
        }
    }
    private void switchCameraCalibrationControlls(int currentUI)
    {
        switch (currentUI)
        {
            case 0:
                //Map calibration request UI
                _playerCamCalib.calibrationControlls = PlayerCameraCalibration.CalibrationControlls.NONE;
                break;
            case 1:
                _playerCamCalib.calibrationControlls = PlayerCameraCalibration.CalibrationControlls.FLOOR;
                break;
            case 2:
                _playerCamCalib.calibrationControlls = PlayerCameraCalibration.CalibrationControlls.MOVEROTATE;
                break;
            case 3:
                //Finish or restart
                _playerCamCalib.calibrationControlls = PlayerCameraCalibration.CalibrationControlls.NONE;
                break;
        }
    }
    
    public void SkipCalibration()
    {
        calibrationFinished();
    }
    public void RestartCalibration()
    {
        hideAllCalibrationUIs();

        _currentUI = 0;
        ShowNextUI();
    }
    private void calibrationFinished()
    {
        //Starts the actual gameplay (Start)
        
        _playerCamCalib.SaveCurrentCalibration();
        hideAllCalibrationUIs();
        disableLaserPointer();
        _playerCamCalib.enabled = false;
        

        proceedToPart1();
        this.enabled = false;
    }

    private void proceedToPart1() 
    {
        //used for subscribing to a listener.
    }

    private void disableLaserPointer()
    {
        uiHelperObj.SetActive(false);
    }
}
