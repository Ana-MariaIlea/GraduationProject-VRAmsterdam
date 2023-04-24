using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitObject : MonoBehaviour
{
    protected float damage;

    public float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
}
