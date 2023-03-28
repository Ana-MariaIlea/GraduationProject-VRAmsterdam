using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCreatureHandler : NetworkBehaviour
{
    //private bool isFireCretureCollected = false;
    //private bool isWaterCretureCollected = false;
    //private bool isEarthCretureCollected = false;
    private NetworkVariable<bool> isFireCretureCollected = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isWaterCretureCollected = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isEarthCretureCollected = new NetworkVariable<bool>(false);

    //private int creaturesColected = 0;
    private NetworkVariable<int> creaturesColected = new NetworkVariable<int>(0);


    public bool IsFireCretureCollected
    {
        get
        {
            return isFireCretureCollected.Value;
        }
    }
    public bool IsWaterCretureCollected
    {
        get
        {
            return isWaterCretureCollected.Value;
        }
    }
    public bool IsEarthCretureCollected
    {
        get
        {
            return isEarthCretureCollected.Value;
        }
    }

    public void CreatureColected(CreatureType type)
    {
        switch (type)
        {
            case CreatureType.Fire:
                isFireCretureCollected.Value = true;
                break;
            case CreatureType.Water:
                isWaterCretureCollected.Value = true;
                break;
            case CreatureType.Earth:
                isEarthCretureCollected.Value = true;
                break;
        }
        creaturesColected.Value++;
        //if(creaturesColected == 3)
        //{
        //    GetComponent<PlayerStateManager>().ChangeStateTo(PlayerStateManager.PlayerState.Part2);
        //}
    }
}
