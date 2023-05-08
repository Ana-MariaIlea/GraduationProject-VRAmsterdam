using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : NetworkBehaviour
{
    [SerializeField] private Button StartGameButton;
    [SerializeField] private GameObject UIElementsPanel;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            UIElementsPanel.SetActive(true);
            StartGameButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        PlayerStateManager.Singleton.StartPart1Server();
        UIElementsPanel.SetActive(false);
    }
}
