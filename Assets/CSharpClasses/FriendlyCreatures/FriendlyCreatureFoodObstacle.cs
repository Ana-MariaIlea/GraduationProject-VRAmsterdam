using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Food Obstacle class inherited from FriendlyCreatureItemObstacle
// </summary>
//------------------------------------------------------------------------------
public class FriendlyCreatureFoodObstacle : FriendlyCreatureItemObstacle
{
    [ServerRpc(RequireOwnership = false)]
    public override void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GetComponentInParent<EarthFriendlyCreature>().CreadureBefriendTransition(serverRpcParams.Receive.SenderClientId);
        GetComponent<BoxCollider>().enabled = false;
        SoundManager.Singleton.PlaySoundAllPlayers(GetComponent<SoundSource>().SoundID, true, serverRpcParams.Receive.SenderClientId);
        ObstacleClearedClientRpc();
    }

    [ClientRpc]
    public void ObstacleClearedClientRpc()
    {
        gameObject.SetActive(false);
    }
}
