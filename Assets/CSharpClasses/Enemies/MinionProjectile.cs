using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinionProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * transform.forward;
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    if (IsServer)
    //    {
    //        body = GetComponent<Rigidbody>();
    //        body.velocity = speed * transform.forward;
    //    }
    //    else
    //    {
    //        GetComponent<SphereCollider>().enabled = false;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //GetComponent<NetworkObject>().Despawn();
            Destroy(this);
        }
    }
}
