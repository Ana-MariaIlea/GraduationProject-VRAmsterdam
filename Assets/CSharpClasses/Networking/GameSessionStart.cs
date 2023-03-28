using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameSessionStart : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] bool isServer = true;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
           // NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            cam.SetActive(false);
            NetworkManager.Singleton.StartClient();
        }

        this.enabled = false;
    }
}
