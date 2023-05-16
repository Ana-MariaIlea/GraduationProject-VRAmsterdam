using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFireProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    [SerializeField] float explosionRadius = 5;

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
            //GetComponent<SphereCollider>().enabled = false;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && IsServer)
        {
            Debug.Log(other.tag);
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
            //Explode();
        }
    }

    private void Explode()
    {
        GetComponent<SphereCollider>().radius = explosionRadius;
        Invoke("DestrouProjectile", 1f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
