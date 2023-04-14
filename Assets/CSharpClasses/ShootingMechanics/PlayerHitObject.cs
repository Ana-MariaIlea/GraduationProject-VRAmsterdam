using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class PlayerHitObject : NetworkBehaviour
{
    protected float damage;
    [SerializeField]protected ulong shooterPlayerID;

    public float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }

    public ulong ShooterPlayerID
    {
        get
        {
            return shooterPlayerID;
        }
        set
        {
            shooterPlayerID = value;
        }
    }
}
