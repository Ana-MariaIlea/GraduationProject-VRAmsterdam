using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private SoundSource playerHitSoundSource;


    private int currentHP;
    private bool isPlayerCoOp = true;

    [SerializeField] private GameObject HealthPanel;
    [SerializeField] private TMP_Text HPText;

    [SerializeField] private Material mat;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHP = maxHP;
        if (!IsServer)
        {
            //GetComponent<BoxCollider>().enabled = false;
            this.enabled = false;
            if (IsOwner)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2Start);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyHitObject")
        {
            PlayerHitServer();

            other.GetComponent<EnemyHitObject>().DestroyProjectileServer();
        }
    }

    [ClientRpc]
    private void PlayerHitClientRPC(float materieanCutoffValue)
    {
        PlayerHitServerRpc();
        //Material Cutoff affect the transparency of the health indicator
        mat.SetFloat("_Cutoff", materieanCutoffValue);
        HPText.text = currentHP.ToString();
    }

    [ServerRpc]
    private void PlayerHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SoundManager.Singleton.PlaySoundAllPlayers(playerHitSoundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
    }

    private void Part2Start()
    {
        GetComponent<BoxCollider>().enabled = true;
        HealthPanel.SetActive(true);
    }

    public void PlayerHitServer(ServerRpcParams serverRpcParams = default)
    {
        currentHP--;
        float materialCutofValue = 1f - currentHP / (float)maxHP;
        PlayerHitClientRPC(materialCutofValue);

        if (currentHP <= 0)
        {
            GetComponentInParent<PlayerVRShooting>().PlayerDieServer();
        }
        else
        {
            GetComponentInParent<PlayerVRShooting>().PlayerHit(currentHP);
        }
    }

    public void RevivePlayerServer()
    {
        currentHP = maxHP;
        RevivePlaterClientRpc();
    }

    [ClientRpc]
    private void RevivePlaterClientRpc()
    {
        mat.SetFloat("_Cutoff", 1f);
        HPText.text = maxHP.ToString();
    }
}
