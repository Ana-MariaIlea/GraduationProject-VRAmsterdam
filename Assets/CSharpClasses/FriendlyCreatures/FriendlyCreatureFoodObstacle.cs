using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCreatureFoodObstacle : FriendlyCreatureItemObstacle
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ObstacleCleared()
    {
        GetComponentInParent<EarthFriendlyCreature>().CreadureBefriendTransition();
    }
}
