using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stream : PlayerHitObject
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            this.enabled = false;
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        switch (other.tag)
        {
            case "ShieldCollider":
                GetComponent<NetworkObject>().Despawn();
                Destroy(this);
                break;
            case "Boss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                other.GetComponent<BossCreature>().DamangeBoss(damage);
                break;
            case "Miniboss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                break;
        }
    }
}
