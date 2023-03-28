using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        base.OnNetworkSpawn();
        creatureType = GetComponentInParent<AbstractFriendlyCreature>().CCreatureType;
    }

    public virtual void ObstacleCleared()
    {
    }
}
