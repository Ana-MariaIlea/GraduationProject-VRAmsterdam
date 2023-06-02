using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FriendlyCreatureFireObstacle : FriendlyCreatureItemObstacle
{
    [ServerRpc(RequireOwnership = false)]
    public override void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (isObstacleClear == false)
        {
            isObstacleClear = true;
            GetComponentInParent<FireFriendlyCreature>().CreadureBefriendTransition(serverRpcParams.Receive.SenderClientId);
            GetComponent<BoxCollider>().enabled = false;
            SoundManager.Singleton.PlaySoundAllPlayers(soundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
            ObstacleClearedClientRpc();
        }
    }

    [ClientRpc]
    public void ObstacleClearedClientRpc()
    {
        gameObject.SetActive(false);
        isObstacleClear = true;
    }
}
