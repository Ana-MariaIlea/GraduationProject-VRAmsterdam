using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Class for starting Server or Clients.
//     Used for testing when there is no lobby system to start the session
// </summary>
//------------------------------------------------------------------------------
public class GameSessionStart : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] bool isServer = true;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            NetworkManager.Singleton.StartServer();
            //NetworkManager.Singleton.StartHost();
        }
        else
        {
            if (cam != null)
                cam.SetActive(false);
            NetworkManager.Singleton.StartClient();
            //NetworkManager.Singleton.StartHost();
        }
    }
    private void Awake()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                isServer = false;
                break;
            case RuntimePlatform.WindowsPlayer:
                isServer = true;
                break;
        }
    }
}
