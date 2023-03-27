using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : PlayerHitObject
{
    private void OnParticleCollision(GameObject other)
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
