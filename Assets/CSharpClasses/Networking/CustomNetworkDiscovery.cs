//Extended by Ana-Maria Ilea from the ExampleNetworkDiscovery

using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkManager))]
public class CustomNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [SerializeField] GameObject cam;
    [SerializeField] bool isServer = true;

    NetworkManager m_NetworkManager;

    public string ServerName = "EnterName";

    private bool m_HasStartedClient = false;

    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();

    public void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
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

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            NetworkManager.Singleton.StartServer();
            StartServer();
        }
        else
        {
            if (cam != null)
                cam.SetActive(false);
            StartClient();
            ClientBroadcast(new DiscoveryBroadcastData());

        }
    }

    public void Update()
    {
        if (!isServer && m_HasStartedClient == false)
        {
            Debug.Log(discoveredServers.Count);
            foreach (var discoveredServer in discoveredServers)
            {
                UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
                transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
                m_NetworkManager.StartClient();
                m_HasStartedClient = true;
                break;
            }
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData()
        {
            ServerName = ServerName,
            Port = ((UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        ServerFound(sender, response);
    }

    void ServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
    }
}
