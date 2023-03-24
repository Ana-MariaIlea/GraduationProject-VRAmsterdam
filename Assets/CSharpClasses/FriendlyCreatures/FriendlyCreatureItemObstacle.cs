using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FriendlyCreatureItemObstacle : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        creatureType = GetComponentInParent<AbstractFriendlyCreature>().CCreatureType;
    }

    public virtual void ObstacleCleared()
    {
    }
}
