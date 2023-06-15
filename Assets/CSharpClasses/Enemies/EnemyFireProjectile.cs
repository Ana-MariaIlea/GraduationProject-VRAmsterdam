//Made by Ana-Maria Ilea

using UnityEngine;

public class EnemyFireProjectile : EnemyHitObject
{
    [SerializeField] float explosionRadius = 5;

    //Funtion not used
    private void Explode()
    {
        GetComponent<SphereCollider>().radius = explosionRadius;
        Invoke("DestrouProjectile", 1f);
    }


}
