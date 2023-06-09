using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecal : MonoBehaviour
{
    public GameObject decalPrefab;  // Prefab with the decal material and mesh
    public float decalSize = 1f;  // Size of the decal
    public float decalLifetime = 10f;  // Lifetime of the decal

    private void OnTriggerEnter(Collider collider)
    {
        var collisionPoint = collider.ClosestPoint(transform.position);
        var collisionNormal = transform.position - collisionPoint;

        Quaternion decalRotation = Quaternion.LookRotation(collisionNormal);

        GameObject decal = Instantiate(decalPrefab, collisionPoint, decalRotation);
        decal.transform.localScale = Vector3.one * decalSize;

        Destroy(decal, decalLifetime);
    }
}
