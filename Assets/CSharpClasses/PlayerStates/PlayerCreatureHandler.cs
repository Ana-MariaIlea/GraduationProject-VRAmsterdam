using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.Burst.Intrinsics.X86;

public class PlayerCreatureHandler : MonoBehaviour
{
    private bool isFireCretureCollected = false;
    private bool isWaterCretureCollected = false;
    private bool isEarthCretureCollected = false;

    private int creaturesColected = 0;

    public bool IsFireCretureCollected
    {
        get
        {
            //Some other code
            return isFireCretureCollected;
        }
    }
    public bool IsWaterCretureCollected
    {
        get
        {
            //Some other code
            return isWaterCretureCollected;
        }
    }
    public bool IsEarthCretureCollected
    {
        get
        {
            //Some other code
            return isEarthCretureCollected;
        }
    }

    public void CreatureColected(CreatureType type)
    {
        switch (type)
        {
            case CreatureType.Fire:
                isFireCretureCollected = true;
                break;
            case CreatureType.Water:
                isWaterCretureCollected = true;
                break;
            case CreatureType.Earth:
                isEarthCretureCollected = true;
                break;
        }
        creaturesColected++;
        if(creaturesColected == 3)
        {
            GetComponent<PlayerStateManager>().ChangeStateTo(PlayerStateManager.PlayerState.Part2);
        }
    }
}
