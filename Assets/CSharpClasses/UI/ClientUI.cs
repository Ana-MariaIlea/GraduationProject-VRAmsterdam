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
}
