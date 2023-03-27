using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PlayerHitObject
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
