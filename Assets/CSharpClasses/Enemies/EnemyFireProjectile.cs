using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFireProjectile : EnemyHitObject
{
    [SerializeField] float explosionRadius = 5;

    private void Explode()
    {
        GetComponent<SphereCollider>().radius = explosionRadius;
        Invoke("DestrouProjectile", 1f);
    }


}
