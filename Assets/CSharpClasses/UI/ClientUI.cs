using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientUI : NetworkBehaviour
{
    [SerializeField] private GameObject UIElementsPanel;
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartClient.AddListener(EndGame);
                PlayerStateManager.Singleton.part1StartClient.AddListener(StartGame);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.AddListener(EndGame);
                PlayerStateManager.Singleton.part1StartServer.AddListener(StartGame);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            base.OnNetworkDespawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartClient.RemoveListener(EndGame);
                PlayerStateManager.Singleton.part1StartClient.RemoveListener(StartGame);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.RemoveListener(EndGame);
                PlayerStateManager.Singleton.part1StartServer.RemoveListener(StartGame);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }
    public void EndGame()
    {
        UIElementsPanel.SetActive(true);
    }

    public void StartGame()
    {
        UIElementsPanel.SetActive(false);
    }
}
