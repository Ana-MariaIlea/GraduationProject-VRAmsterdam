using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Projectile : PlayerHitObject
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    [SerializeField] GameObject decalPrefab;
    public float decalSize = 1f;  // Size of the decal

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
        if (other.tag == "Boundary")
        {
            SpawnDecal(other);
        }
        if (other.tag != "Player" && other.tag != "ChargingStation")
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(this);
        }
    }

    private void PlayerVSPlayerTriggerHandling(Collider other)
    {
        //if(other.tag == opposingTeamTag)
        //{
        //    //Increase score and damage player
        //    ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
        //    bool otherHP = other.GetComponent<PlayerVRLifeSystem>().PlayerHitServer();
        //    if (otherHP) 
        //    {
        //        ScoreSystemManager.Singleton.KillAddedToPlayer(shooterPlayerID);
        //    }
        //}
        if (other.tag == "Boundary")
        {
            SpawnDecal(other);
        }
        if (other.tag != "ChargingStation" && other.tag!="Team1" && other.tag != "Team2" && other.tag != "Player")
        {
            //add decal
            GetComponent<NetworkObject>().Despawn();
            Destroy(this);
        }

        
    }
    private void SpawnDecal(Collider other)
    {
        if (decalPrefab != null)
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            var collisionNormal = transform.position - collisionPoint;

            Quaternion decalRotation = Quaternion.LookRotation(collisionNormal);

            GameObject decal = Instantiate(decalPrefab, transform.position, transform.rotation);
            decal.transform.localScale = Vector3.one * decalSize;
            decal.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
