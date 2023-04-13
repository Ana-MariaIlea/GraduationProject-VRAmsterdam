using System.Collections;
using System.Collections.Generic;
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
        //Also do damage

        AddScore(other.tag);
    }

    private void AddScore(string objectTag)
    {
        switch (objectTag)
        {
            case "Boss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                break;
            case "Miniboss":
                ScoreSystemManager.Singleton.ScoreAddedToPlayer(shooterPlayerID);
                break;
        }
    }
}
