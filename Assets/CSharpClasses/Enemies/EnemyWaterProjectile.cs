using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyWaterProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
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
        if (other.tag != "Enemy")
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
