using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVREnding : NetworkBehaviour
{
    [SerializeField] private GameObject endingUIPanel;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.endingStartClient.AddListener(GameEnd);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
        
    }

    private void GameEnd()
    {
        endingUIPanel.SetActive(true);
    }
}
