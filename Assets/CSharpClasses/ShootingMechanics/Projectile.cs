using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PlayerHitObject
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;

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
        //Also do damage
        switch (other.tag)
        {
            case "Boss":
                ScoreSystemManager.Instance.ScoreAddedToPlayer(shooterPlayerID);
                break;
            case "Miniboss":
                ScoreSystemManager.Instance.ScoreAddedToPlayer(shooterPlayerID);
                break;
        }
    }
}
