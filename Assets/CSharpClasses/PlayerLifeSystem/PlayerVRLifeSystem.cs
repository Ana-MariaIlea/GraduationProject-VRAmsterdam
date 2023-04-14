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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChargingStation")
        {
            RevivePlayerServerRpc();
        }
        else if (other.tag == "PlayerHitObject")
        {
            PlayerHitServerRpc();
        }
    }

    [ServerRpc]
    private void PlayerHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP--;
        if (currentHP <= 0)
        {
            PlayerDie(serverRpcParams);
        }
        else
        {
            GetComponent<PlayerVRShooting>().PlayerHit(currentHP);
        }
    }

    [ServerRpc]
    private void RevivePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        currentHP = maxHP;
    }
    private void PlayerDie(ServerRpcParams serverRpcParams = default)
    {
        GetComponent<PlayerVRShooting>().PlayerDieClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { serverRpcParams.Receive.SenderClientId } } });
    }
}
