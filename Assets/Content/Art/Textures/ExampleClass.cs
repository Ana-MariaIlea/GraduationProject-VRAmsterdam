using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleClass : MonoBehaviour
{
    public ParticleSystem part_left;
    public ParticleSystem part_right;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part_left = GetComponentInChildren<ParticleSystem>();
        part_right = GetComponentInChildren<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part_left.GetCollisionEvents(other, collisionEvents) + part_right.GetCollisionEvents(other, collisionEvents);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        int i = 0;
        Debug.Log(numCollisionEvents);

        while (i < numCollisionEvents)
        {
            if (rb)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 10;
                rb.AddForce(force);
            }
            i++;
        }
    }
}