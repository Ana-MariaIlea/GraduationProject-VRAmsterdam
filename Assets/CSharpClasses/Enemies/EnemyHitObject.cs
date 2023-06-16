//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//      Base class for enemy projectile
// </summary>
//------------------------------------------------------------------------------
public class EnemyHitObject : NetworkBehaviour
{
    [SerializeField] protected Rigidbody body;
    [SerializeField] protected float speed = 1;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            body = GetComponent<Rigidbody>();
            body.velocity = speed * transform.forward;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsRightTrigger(other.tag) && IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    private bool IsRightTrigger(string tag)
    {
        if (tag != "Boss" && tag != "Minion" && tag != "ShieldBoss" && tag != "ChargingStation" && tag != "Player")
            return true;
        return false;
    }

    public void DestroyProjectileServer()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
