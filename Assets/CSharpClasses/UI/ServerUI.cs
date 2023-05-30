using System.Collections;
using System.Collections.Generic;
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
    }

    public void StartPlayerCoOpGame()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.StartPart1Server(true);
            UIElementsPanel.SetActive(false);
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
