using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
