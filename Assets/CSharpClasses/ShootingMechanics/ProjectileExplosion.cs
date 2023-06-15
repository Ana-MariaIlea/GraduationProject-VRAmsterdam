//Made by Ana-Maria Ilea

using System.Collections;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//      Effect for the projectile colision with a wall or boundery
// </summary>
//------------------------------------------------------------------------------
public class ProjectileExplosion : NetworkBehaviour
{
    [SerializeField] private ParticleSystem explosionEffect1;
    [SerializeField] private ParticleSystem explosionEffect2;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            explosionEffect1.Play();
            explosionEffect2.Play();
        }

        if (IsServer)
        {
            StartCoroutine(PlayEffectAndDestroy());
        }
    }

    private IEnumerator PlayEffectAndDestroy()
    {
        yield return new WaitForSeconds(explosionEffect1.main.duration);

        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

}
