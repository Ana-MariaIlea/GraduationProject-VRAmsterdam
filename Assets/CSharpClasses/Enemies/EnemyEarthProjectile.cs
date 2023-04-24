using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEarthProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    [SerializeField] float explosionRadius = 5;
    [SerializeField] GameObject earthColumns;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * transform.forward;
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
        //creature.GetComponent<NetworkObject>().Spawn(true);

        GetComponent<SphereCollider>().enabled = false;
        body.velocity = Vector3.zero;
        Invoke("DestrouProjectile", 4f);
    }

    private void DestrouProjectile()
    {
        //GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
