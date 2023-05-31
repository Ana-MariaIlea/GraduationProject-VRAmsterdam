using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Script used to assign the player to different manager list (E.g., scoring) 
// </summary>
//------------------------------------------------------------------------------
public class PlayerAssignmentHandler : NetworkBehaviour
{
    [SerializeField] private Material team1Material;
    [SerializeField] private Material team2Material;
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    [SerializeField] private GameObject playerVRLiveObject;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            AddPlayerCreaturesServerRPC();
            AddPlayerToScoringServerRPC();
            AssignStreamShooterIDServerRpc();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.tag == "TeamAssignment")
        {
            ChangePlayerVisualsClientRpc(other.GetComponent<TeamAssignmentSpot>().team);
            switch (other.GetComponent<TeamAssignmentSpot>().team)
            {
                case PossibleTeams.Team1:
                    playerMesh.material = team1Material;
                    gameObject.tag = "Team1";
                    playerVRLiveObject.tag = "Team1";
                    break;
                case PossibleTeams.Team2:
                    playerMesh.material = team2Material;
                    gameObject.tag = "Team2";
                    playerVRLiveObject.tag = "Team2";
                    break;
            }
            
        }
    }

    [ClientRpc]
    private void ChangePlayerVisualsClientRpc(PossibleTeams team)
    {
        switch (team)
        {
            case PossibleTeams.Team1:
                playerMesh.material = team1Material;
                gameObject.tag = "Team1";
                break;
            case PossibleTeams.Team2:
                playerMesh.material = team2Material;
                gameObject.tag = "Team2";
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        RemovePlayerCreaturesServerRPC();
        RemovePlayerToScoringServerRPC();
    }

    [ServerRpc]
    private void AddPlayerCreaturesServerRPC(ServerRpcParams serverRpcParams = default)
    {
        PlayerCreatureHandler.Singleton.AddEmptyPlayerStructure(serverRpcParams);
    }

    [ServerRpc]
    private void RemovePlayerCreaturesServerRPC(ServerRpcParams serverRpcParams = default)
    {
        PlayerCreatureHandler.Singleton.RemovePlayerStructure(serverRpcParams);
    }

    [ServerRpc]
    private void AddPlayerToScoringServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ScoreSystemManager.Singleton.NewPlayerConnected(serverRpcParams);
    }

    [ServerRpc]
    private void RemovePlayerToScoringServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ScoreSystemManager.Singleton.PlayerDisonnected(serverRpcParams);
    }

    [ServerRpc]
    private void AssignStreamShooterIDServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Stream[] streamsObjetcs = GetComponentsInChildren<Stream>();
        foreach (var stream in streamsObjetcs)
        {
            stream.ShooterPlayerID = serverRpcParams.Receive.SenderClientId;
        }
    }
}
