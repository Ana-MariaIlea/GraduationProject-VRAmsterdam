using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class PlayerHitObject : NetworkBehaviour
{
    protected float damage;
    protected ulong shooterPlayerID;
    protected bool isPlayerCoOp = true;
    protected string opposingTeamTag = "Player";

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

    public bool IsPlayerCoOp
    {
        get
        {
            return isPlayerCoOp;
        }
        set
        {
            isPlayerCoOp = value;
        }
    }

    public string OpposingTeamTag
    {
        get
        {
            return opposingTeamTag;
        }
        set
        {
            opposingTeamTag = value;
        }
    }

    public void DestroyProjectileServer()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
