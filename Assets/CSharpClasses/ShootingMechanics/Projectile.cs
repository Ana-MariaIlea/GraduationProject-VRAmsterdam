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
        //Also do damage
        switch (other.tag)
        {
            case "ShieldCollider":
                GetComponent<NetworkObject>().Despawn();
                Destroy(this);
                break;
        }

        AddScore(other.tag);
    }

    private void AddScore(string objectTag)
    {
        switch (objectTag)
        {
            case "Boss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                break;
            case "Miniboss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                break;
        }
    }
}
