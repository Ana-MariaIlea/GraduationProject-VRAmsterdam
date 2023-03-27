using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVRLifeSystem : MonoBehaviour
{
    [SerializeField] int maxHP = 10;

    private int currentHP;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChargingStation")
        {
            currentHP = maxHP;
        }
        else if (other.tag == "PlayerHitObject")
        {
            currentHP--;
            if(currentHP <= 0)
            {
                GetComponent<PlayerVRShooting>().PlayerDie();
            }
            else
            {
                GetComponent<PlayerVRShooting>().PlayerHit(currentHP);
            }
        }
    }
}
