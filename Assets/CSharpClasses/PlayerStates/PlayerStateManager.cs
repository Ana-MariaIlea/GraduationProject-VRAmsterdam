using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager : NetworkBehaviour
{
    public static PlayerStateManager Singleton;

    [HideInInspector] public UnityEvent part1StartClient;
    [HideInInspector] public UnityEvent part1StartServer;

    [HideInInspector] public UnityEvent part2StartClient;
    [HideInInspector] public UnityEvent part2StartServer;

    [HideInInspector] public UnityEvent endingStartServer;
    [HideInInspector] public UnityEvent endingStartClient;

    public enum PlayerState
    {
        Part1,
        Part2,
        Part3
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public void StartPart1Server()
    {
        StartPart1ClientRpc();
        part1StartServer?.Invoke();
    }

    [ClientRpc]
    private void StartPart1ClientRpc()
    {
        part1StartClient?.Invoke();
    }

    public void StartPart2Server()
    {
        StartPart2ClientRpc();
        part2StartServer?.Invoke();
    }

    [ClientRpc]
    private void StartPart2ClientRpc()
    {
        part2StartClient?.Invoke();
    }

    public void GameEnd()
    {
        endingStartClient?.Invoke();
    }
}
