using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager : NetworkBehaviour
{
    public static PlayerStateManager Singleton { get; private set; }

    [HideInInspector] public UnityEvent part1StartClient;
    [HideInInspector] public UnityEvent part1StartServer;

    [HideInInspector] public UnityEvent part2PlayerCoOpStartClient;
    [HideInInspector] public UnityEvent part2PlayerCoOpStartServer;

    [HideInInspector] public UnityEvent part2PlayerVsPlayerStartClient;
    [HideInInspector] public UnityEvent part2PlayerVsPlayerStartServer;

    [HideInInspector] public UnityEvent endingStartServer;
    [HideInInspector] public UnityEvent endingStartClient;

    private bool isPart1Triggered = false;
    private bool isPart2Triggered = false;
    private bool isEndingTriggered = false;

    private bool isPlayerCoOp = true;

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


    public void StartPart1Server(bool isPlayerCoOp)
    {
        if (!isPart1Triggered)
        {
            StartPart1ClientRpc();
            part1StartServer?.Invoke();
            this.isPlayerCoOp = isPlayerCoOp;
            isPart1Triggered = true;
        }
    }

    [ClientRpc]
    private void StartPart1ClientRpc()
    {
        part1StartClient?.Invoke();
    }

    public void StartPart2Server()
    {
        if (isPlayerCoOp)
        {
            StartPart2PlayerCoOpServer();
        }
        else
        {
            StartPart2PlayerVsPlayerServer();
        }
    }
    public void StartPart2PlayerCoOpServer()
    {
        if (!isPart2Triggered)
        {
            StartPart2PlayerCoOpClientRpc();
            part2PlayerCoOpStartServer?.Invoke();
            isPart2Triggered = true;
        }
    }

    [ClientRpc]
    private void StartPart2PlayerCoOpClientRpc()
    {
        part2PlayerCoOpStartClient?.Invoke();
    }

    public void StartPart2PlayerVsPlayerServer()
    {
        if (!isPart2Triggered)
        {
            StartPart2PlayerVsPlayerClientRpc();
            part2PlayerVsPlayerStartServer?.Invoke();
            isPart2Triggered = true;
        }
    }

    [ClientRpc]
    private void StartPart2PlayerVsPlayerClientRpc()
    {
        part2PlayerVsPlayerStartClient?.Invoke();
    }

    public void GameEndServer()
    {
        if (!isEndingTriggered)
        {
            endingStartServer?.Invoke();
            GameEndClientRpc();
            isEndingTriggered = true;
        }
    }

    [ClientRpc]
    private void GameEndClientRpc()
    {
        endingStartClient?.Invoke();
    }
}
