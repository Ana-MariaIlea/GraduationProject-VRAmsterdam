using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCreatureSpatialObstacle : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void ObstacleCleared()
    {
        GetComponent<WaterFriendlyCreature>().CreadureBefriendTransition();
    }
}
