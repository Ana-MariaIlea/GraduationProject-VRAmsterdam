using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyEarthProjectile : EnemyHitObject
{
    [SerializeField] float explosionRadius = 5;
    [SerializeField] GameObject earthColumns;
    private void Explode()
    {
        GameObject creature = Instantiate(earthColumns, transform.position, Quaternion.identity, transform);
        creature.GetComponent<NetworkObject>().Spawn(true);

        GetComponent<SphereCollider>().enabled = false;
        body.velocity = Vector3.zero;
        Invoke("DestrouProjectile", 4f);
    }
}
