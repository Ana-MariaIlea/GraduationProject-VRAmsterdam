using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Player life system class. This class handles collision with enemy projectiles
//     PlayerHit and PlayerDie are called when a collision happens
// </summary>
//------------------------------------------------------------------------------
public class PlayerVRLifeSystem : NetworkBehaviour
{
    [SerializeField] int maxHP = 10;

    private int currentHP;
    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHP = maxHP;
        if (IsServer)
        { if (PlayerStateManager.Singleton)
            {
                //PlayerStateManager.Singleton.part2StartServer.AddListener(Part2Start);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
            this.enabled = false;
        }
        else if (IsClient && IsOwner)
                if (PlayerStateManager.Singleton)
                {
                    PlayerStateManager.Singleton.part2StartClient.AddListener(Part2Start);
                }
                else
                {
                    Debug.LogError("No PlayerStateManager in the scene");
                }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyHitObject")
        {
            Debug.Log("Life system trigger enter");
            PlayerHitServerRpc();
            other.GetComponent<EnemyFireProjectile>().DestroyProjectileServerRpc();
        }
    }

    private void Part2Start()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    [ServerRpc]
    public void PlayerHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP--;
        if (currentHP <= 0)
        {
            GetComponentInParent<PlayerVRShooting>().PlayerDieClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { serverRpcParams.Receive.SenderClientId } } });
        }
        else
        {
            GetComponentInParent<PlayerVRShooting>().PlayerHit(currentHP);
        }
    }

    [ServerRpc]
    public void RevivePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP = maxHP;
    }
}
