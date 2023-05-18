using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyHitObject : NetworkBehaviour
{
    [SerializeField] protected Rigidbody body;
    [SerializeField] protected float speed = 1;
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
        if (other.tag != "Enemy" && other.tag != "Player" && IsServer)
        {
            Debug.Log(other.tag);
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
