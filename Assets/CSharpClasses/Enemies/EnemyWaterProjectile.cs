using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaterProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        //GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
