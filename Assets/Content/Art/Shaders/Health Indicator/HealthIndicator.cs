using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    private Material mat; // The material of the health indicator
    void Start()
    {
        mat = GetComponent<Renderer>().material;
       //FindObjectOfType<PlayerVRLifeSystem>().PlayerHitServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
