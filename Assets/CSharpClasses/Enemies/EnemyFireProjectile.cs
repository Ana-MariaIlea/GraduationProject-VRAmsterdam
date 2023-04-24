using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyFireProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    [SerializeField] float explosionRadius = 5;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void Explode()
    {
        GetComponent<SphereCollider>().radius = explosionRadius;
        Invoke("DestrouProjectile", 1f);
    }

    private void DestrouProjectile()
    {
        //GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
