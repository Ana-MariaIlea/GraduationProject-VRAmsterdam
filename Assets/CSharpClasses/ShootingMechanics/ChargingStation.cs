using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    [SerializeField] private CreatureType creatureType;

    public CreatureType CCreatureType
    {
        get
        {
            //Some other code
            return creatureType;
        }
    }
}
