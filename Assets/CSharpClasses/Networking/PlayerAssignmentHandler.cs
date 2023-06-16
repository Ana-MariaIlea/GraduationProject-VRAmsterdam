//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;
using TMPro;

//------------------------------------------------------------------------------
// </summary>
//     Script used to assign the player to different manager list (E.g., scoring) 
// </summary>
//------------------------------------------------------------------------------
public class PlayerAssignmentHandler : NetworkBehaviour
{
    [SerializeField] private Material team1Material;
    [SerializeField] private Material team2Material;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    [SerializeField] private GameObject playerVRLiveObject;
    [SerializeField] private TMP_Text PlayerNameText;
    private NetworkVariable<ulong> playerNameID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            if (IsOwner)
            {
                AddPlayerCreaturesServerRPC();
                AddPlayerToScoringServerRPC();
                AssignPlayerNameServerRpc();
            }
            else
            {
                AssignPlayerNameClient();
                
            }
            this.enabled = false;
        }

        if (IsServer)
        {
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.AddListener(EndGameServer);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void EndGameServer()
    {
        playerMesh.material = defaultMaterial;
        gameObject.tag = "Player";
        playerVRLiveObject.tag = "Player";
        EndGameClientRpc();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        playerMesh.material = defaultMaterial;
        gameObject.tag = "Player";
        playerVRLiveObject.tag = "Player";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.tag == "TeamAssignment")
        {
            Debug.Log("team assignment");
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

    [ServerRpc]
    private void AssignPlayerNameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        PlayerNameText.text = "Player " + serverRpcParams.Receive.SenderClientId.ToString();
        playerNameID.Value = serverRpcParams.Receive.SenderClientId;
        AssignPlayerNameClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void AssignPlayerNameClientRpc(ulong playerID)
    {
        PlayerNameText.text = "Player " + playerID.ToString();
    }

    private void AssignPlayerNameClient()
    {
        PlayerNameText.text = "Player " + playerNameID.Value.ToString();
    }

    [ClientRpc]
    private void ChangePlayerVisualsClientRpc(PossibleTeams team)
    {
        switch (team)
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
        AddPlayerToPlayerVsPlayerServerRPC();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        RemovePlayerCreaturesServerRPC();
        RemovePlayerToScoringServerRPC();

        if (IsServer)
        {
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.RemoveListener(EndGameServer);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    [ServerRpc]
    private void AddPlayerToPlayerVsPlayerServerRPC(ServerRpcParams serverRpcParams = default)
    {
        PlayerVsPlayerTimer.Singleton.NewPlayerConnected(serverRpcParams);
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
}
