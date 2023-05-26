using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FriendlyCreatureFireObstacle : FriendlyCreatureItemObstacle
{
    [ServerRpc(RequireOwnership = false)]
    public override void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GetComponentInParent<FireFriendlyCreature>().CreadureBefriendTransition(serverRpcParams.Receive.SenderClientId);
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
