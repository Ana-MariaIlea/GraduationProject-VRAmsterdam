using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FriendlyCreatureItemObstacle : MonoBehaviour
{
    [SerializeField] private ItemID obstacleItemID;

    public ItemID ObstacleItemID
    {
        get
        {
            //Some other code
            return obstacleItemID;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void ObstacleCleared()
    {
    }
}
