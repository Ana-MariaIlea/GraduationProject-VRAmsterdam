using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerUI : NetworkBehaviour
{
    [SerializeField] private Button StartGameButton;
    [SerializeField] private GameObject UIElementsPanel;
    [SerializeField] private GameObject EventSystem;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            UIElementsPanel.SetActive(true);
            EventSystem.SetActive(true);
            StartGameButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.StartPart1Server();
            UIElementsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }
}
