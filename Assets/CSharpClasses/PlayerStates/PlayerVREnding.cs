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
        PlayerCreatureHandler.Singleton.endingStartClient.AddListener(GameEnd);
    }

    private void GameEnd()
    {
        endingUIPanel.SetActive(true);
    }
}
