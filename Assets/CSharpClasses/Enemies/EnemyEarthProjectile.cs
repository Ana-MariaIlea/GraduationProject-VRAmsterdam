using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyEarthProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    [SerializeField] float explosionRadius = 5;
    [SerializeField] GameObject earthColumns;

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
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject creature = Instantiate(earthColumns, transform.position, Quaternion.identity, transform);
        creature.GetComponent<NetworkObject>().Spawn(true);

        GetComponent<SphereCollider>().enabled = false;
        body.velocity = Vector3.zero;
        Invoke("DestrouProjectile", 4f);
    }

    private void DestrouProjectile()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
