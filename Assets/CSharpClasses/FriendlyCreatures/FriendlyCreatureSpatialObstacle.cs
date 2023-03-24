using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCreatureSpatialObstacle : FriendlyCreatureItemObstacle
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ObstacleCleared()
    {
        GetComponentInParent<WaterFriendlyCreature>().CreadureBefriendTransition();
        GetComponent<BoxCollider>().enabled = false;
    }
}
