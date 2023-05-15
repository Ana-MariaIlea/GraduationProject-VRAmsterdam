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
        if (PlayerStateManager.Singleton)
        {
            //PlayerStateManager.Singleton.part2StartServer.AddListener(Part2Start);
            PlayerStateManager.Singleton.part2StartClient.AddListener(Part2Start);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChargingStation")
        {
            RevivePlayerServerRpc();
        }
        else if (other.tag == "EnemyHitObject")
        {
            PlayerHitServerRpc();
        }
    }

    private void Part2Start()
    {
        GetComponent<MeshCollider>().enabled = true;
    }

    [ServerRpc]
    private void PlayerHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP--;
        if (currentHP <= 0)
        {
            GetComponent<PlayerVRShooting>().PlayerDieClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { serverRpcParams.Receive.SenderClientId } } });
        }
        else
        {
            GetComponentInParent<PlayerVRShooting>().PlayerHit(currentHP);
        }
    }

    [ServerRpc]
    private void RevivePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP = maxHP;
    }
}
