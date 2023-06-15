//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Food(Earth creature) Obstacle class inherited from FriendlyCreatureItemObstacle
// </summary>
//------------------------------------------------------------------------------
public class FriendlyCreatureFoodObstacle : FriendlyCreatureItemObstacle
{
    [ServerRpc(RequireOwnership = false)]
    public override void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (isObstacleClear == false)
        {
            isObstacleClear = true;
            GetComponentInParent<EarthFriendlyCreature>().CreadureBefriendTransition(serverRpcParams.Receive.SenderClientId);
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
