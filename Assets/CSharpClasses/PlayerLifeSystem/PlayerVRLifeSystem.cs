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

    [SerializeField]private GameObject HPText;

    [SerializeField]private Material mat;

    private void Start()
    {
        //Should put it in OnNetworkSpawn I know...
        mat = GetComponent<Renderer>().material;
        currentHP = maxHP;
        HPText = GameObject.Find("HPText");
        HPText.GetComponent<TMPro.TextMeshProUGUI>().text = "HP: " + currentHP;
    }


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
        
        //Material Cutoff affect the transparency of the health indicator
        mat.SetFloat("_Cutoff", 1f - currentHP / (float)maxHP);
        
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
