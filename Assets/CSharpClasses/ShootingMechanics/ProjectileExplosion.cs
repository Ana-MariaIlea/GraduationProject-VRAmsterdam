using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileExplosion : NetworkBehaviour
{
    [SerializeField] private ParticleSystem explosionEffect;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            explosionEffect.Play();
        }

        if (IsServer)
        {
            StartCoroutine(PlayEffectAndDestroy());
        }
    }

    private IEnumerator PlayEffectAndDestroy()
    {
        yield return new WaitForSeconds(explosionEffect.main.duration);

        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

}
