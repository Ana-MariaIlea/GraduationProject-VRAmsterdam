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
    [ServerRpc]
    public override void ObstacleClearedServerRpc()
    {
        GetComponentInParent<EarthFriendlyCreature>().CreadureBefriendTransition();
        GetComponent<BoxCollider>().enabled = false;
        ObstacleClearedClientRpc();
    }

    [ClientRpc]
    public void ObstacleClearedClientRpc()
    {
        gameObject.SetActive(false);
    }
}
