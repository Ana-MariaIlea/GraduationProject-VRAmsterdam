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
            PlayerStateManager.Singleton.endingStartClient.AddListener(EndGame);
        }
    }

    public void EndGame()
    {
        UIElementsPanel.SetActive(true);
    }
}
