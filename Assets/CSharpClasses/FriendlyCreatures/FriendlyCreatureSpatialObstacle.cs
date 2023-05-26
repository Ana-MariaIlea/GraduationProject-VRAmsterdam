using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Spatial Obstacle class inherited from FriendlyCreatureItemObstacle
// </summary>
//------------------------------------------------------------------------------
public class FriendlyCreatureSpatialObstacle : FriendlyCreatureItemObstacle
{
    [ServerRpc(RequireOwnership = false)]
    public override void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Water obstacle clear server rpc"); 
        GetComponentInParent<WaterFriendlyCreature>().CreadureBefriendTransition(serverRpcParams.Receive.SenderClientId);
        GetComponent<BoxCollider>().enabled = false;
        SoundManager.Singleton.PlaySoundAllPlayers(GetComponent<SoundSource>().SoundID);
        ObstacleClearedClientRpc();
    }

    [ClientRpc]
    public void ObstacleClearedClientRpc()
    {
        Debug.Log("Water obstacle clear client rpc");

        gameObject.SetActive(false);
    }
}
