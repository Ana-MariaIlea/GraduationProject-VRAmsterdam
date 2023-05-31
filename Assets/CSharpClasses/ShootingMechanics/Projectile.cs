using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : PlayerHitObject
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            body = GetComponent<Rigidbody>();
            body.velocity = speed * transform.forward;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerCoOp)
        {
            PlayerCoOpTriggerHandling(other);
        }
        else
        {
            PlayerVSPlayerTriggerHandling(other);
        }
    }


    private void PlayerCoOpTriggerHandling(Collider other)
    {
        switch (other.tag)
        {
            case "Boss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                other.GetComponent<BossCreature>().DamangeBoss(damage);
                break;
            case "Minion":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                other.GetComponent<MinionCreature>().DamangeMinion(damage);
                break;
        }
        if (other.tag != "Player" && other.tag != "ChargingStation")
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(this);
        }
    }

    private void PlayerVSPlayerTriggerHandling(Collider other)
    {
        if(other.tag == "Player")
        {
            //Increase score
            ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
        }
        if (other.tag != "ChargingStation")
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(this);
        }
    }
}
