using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerUI : NetworkBehaviour
{
    [SerializeField] private Button StartPlayerCoOpGameButton;
    [SerializeField] private Button StartPlayerVsPlayerGameButton;
    [SerializeField] private GameObject UIElementsPanel;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private TMP_Text clientConnectedText;
    [SerializeField] private TMP_Text possibleEorrorText;
    private GameMode gameMode = GameMode.NoneSelected;

    public enum GameMode
    {
        NoneSelected,
        CoOp,
        PvP
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            UIElementsPanel.SetActive(true);
            EventSystem.SetActive(true);
            StartPlayerCoOpGameButton.onClick.AddListener(StartPlayerCoOpGame);
            StartPlayerVsPlayerGameButton.onClick.AddListener(StartPlayerVsPlayerGame);
        }
        else
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        clientConnectedText.text = NetworkManager.Singleton.ConnectedClientsIds.Count.ToString();
    }

    public void ChooseGameMode(int index)
    {
        switch (index)
        {
            case 0:
                gameMode = GameMode.NoneSelected;
                break;
            case 1:
                gameMode = GameMode.CoOp;
                possibleEorrorText.text = string.Empty;
                break;
            case 2:
                gameMode = GameMode.PvP;
                possibleEorrorText.text = string.Empty;
                break;
        }
    }

    public void StartGame()
    {
        switch (gameMode)
        {
            case GameMode.NoneSelected:
                possibleEorrorText.text = "No Game Mode Selected. Please choose a game mode.";
                break;
            case GameMode.CoOp:
                StartPlayerCoOpGame();
                break;
            case GameMode.PvP:
                StartPlayerVsPlayerGame();
                break;
            default:
                break;
        }
    }

    public void StartPlayerCoOpGame()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.StartPart1Server(true);
            UIElementsPanel.SetActive(false);
            this.enabled = false;
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }

    public void StartPlayerVsPlayerGame()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.StartPart1Server(false);
            UIElementsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }
}
