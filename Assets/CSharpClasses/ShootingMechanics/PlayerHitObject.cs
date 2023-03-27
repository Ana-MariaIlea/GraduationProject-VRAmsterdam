using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerHitObject : MonoBehaviour
{
    protected float damage;
    protected int shooterPlayerID;

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

    public int ShooterPlayerID
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
