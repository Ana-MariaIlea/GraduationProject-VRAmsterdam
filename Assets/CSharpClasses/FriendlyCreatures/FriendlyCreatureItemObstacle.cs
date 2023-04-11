using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Obstacle needed to be cleared for the creature to be friended
//     Base class for all the obstacles
// </summary>
//------------------------------------------------------------------------------
public abstract class FriendlyCreatureItemObstacle : NetworkBehaviour
{
    [SerializeField] private ItemID obstacleItemID;

    private CreatureType creatureType;
    public ItemID ObstacleItemID
    {
        get
        {
            //Some other code
            return obstacleItemID;
        }
    }

    public CreatureType CCreatureType
    {
        get
        {
            //Some other code
            return creatureType;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            creatureType = GetComponentInParent<AbstractFriendlyCreature>().CCreatureType;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void ObstacleClearedServerRpc(ServerRpcParams serverRpcParams = default)
    {
    }
}
