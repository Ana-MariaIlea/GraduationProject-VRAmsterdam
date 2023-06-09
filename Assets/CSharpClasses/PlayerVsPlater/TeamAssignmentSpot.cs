using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamAssignmentSpot : NetworkBehaviour
{
    public PossibleTeams team;
    [SerializeField] private GameObject visuals;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            PlayerStateManager.Singleton.part2PlayerVsPlayerPreStartServer.AddListener(StartPrePart2PlayerVSPlayerServer);
            PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.AddListener(StartPart2PlayerVSPlayerServer);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            PlayerStateManager.Singleton.part2PlayerVsPlayerPreStartServer.RemoveListener(StartPrePart2PlayerVSPlayerServer);
            PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.RemoveListener(StartPart2PlayerVSPlayerServer);
        }
    }

    private void StartPrePart2PlayerVSPlayerServer()
    {
        GetComponent<BoxCollider>().enabled = true;
        visuals.SetActive(true);
        StartPrePart2PlayerVSPlayerServerClientRpc();
    }
    [ClientRpc]
    private void StartPrePart2PlayerVSPlayerServerClientRpc()
    {
        GetComponent<BoxCollider>().enabled = true;
        visuals.SetActive(true);
    }

    private void StartPart2PlayerVSPlayerServer()
    {
        GetComponent<BoxCollider>().enabled = false;
        visuals.SetActive(false);
        StartPart2PlayerVSPlayerServerClientRpc();
    }

    [ClientRpc]
    private void StartPart2PlayerVSPlayerServerClientRpc()
    {
        GetComponent<BoxCollider>().enabled = false;
        visuals.SetActive(false);
    }
}

public enum PossibleTeams
{
    Team1,
    Team2
}
