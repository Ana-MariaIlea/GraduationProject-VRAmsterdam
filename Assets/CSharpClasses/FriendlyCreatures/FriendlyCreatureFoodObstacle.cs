using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Food Obstacle class inherited from FriendlyCreatureItemObstacle
// </summary>
//------------------------------------------------------------------------------
public class FriendlyCreatureFoodObstacle : FriendlyCreatureItemObstacle
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ObstacleCleared()
    {
        GetComponentInParent<EarthFriendlyCreature>().CreadureBefriendTransition();
        GetComponent<BoxCollider>().enabled = false;
    }
}
