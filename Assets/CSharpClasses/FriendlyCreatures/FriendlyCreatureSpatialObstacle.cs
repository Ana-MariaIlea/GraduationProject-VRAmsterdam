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
    [ServerRpc]
    public override void ObstacleClearedServerRpc()
    {
        GetComponentInParent<WaterFriendlyCreature>().CreadureBefriendTransition();
        GetComponent<BoxCollider>().enabled = false;
        ObstacleClearedClientRpc();
    }

    [ClientRpc]
    public void ObstacleClearedClientRpc()
    {
        gameObject.SetActive(false);
    }
}
