using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Script used to assign the player to different manager list (E.g., scoring) 
// </summary>
//------------------------------------------------------------------------------
public class PlayerAssignmentHandler : NetworkBehaviour
{
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
