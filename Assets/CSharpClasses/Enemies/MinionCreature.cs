using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static BossCreature;

public class MinionCreature : MonoBehaviour
{
    [SerializeField] private float MaxHealth = 100;
    float health;

    private CreatureType creatureType;
    public CreatureType CCreatureType
    {
        get { return creatureType; }
        set { creatureType = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
    }

    private void InitiallizeMinion()
    {

    } 

    // Update is called once per frame
    void Update()
    {

    }

    public void DamangeMinion(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            MinionDie();
            return;
        }
    }

    private void MinionDie()
    {

    }
}
