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
                if (PlayerStateManager.Singleton)
                {
                    PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2PlayerVSPlayerStart);
                }
                else
                {
                    Debug.LogError("No PlayerStateManager in the scene");
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerCoOp)
        {
            if (other.tag == "EnemyHitObject")
            {
                PlayerHitServer();

                other.GetComponent<EnemyHitObject>().DestroyProjectileServer();
            }
        }
        else
        {
            if (other.tag == "PlayerHitObject")
            {
                if (other.GetComponent<Projectile>().OpposingTeamTag == gameObject.tag)
                {
                    ScoreSystemManager.Singleton.ScoreAddedToPlayer(other.GetComponent<Projectile>().ShooterPlayerID);
                    bool otherHP = PlayerHitServer();
                    if (otherHP)
                    {
                        ScoreSystemManager.Singleton.KillAddedToPlayer(other.GetComponent<Projectile>().ShooterPlayerID);
                    }

                    other.GetComponent<PlayerHitObject>().DestroyProjectileServer();
                }
            }
        }
    }

    private void Part2PlayerVSPlayerStart()
    {
        isPlayerCoOp = false;
        Part2PlayerVSPlayerStartServerRpc();
    }

    [ServerRpc]
    private void Part2PlayerVSPlayerStartServerRpc()
    {
        isPlayerCoOp = false;
    }

    [ClientRpc]
    private void PlayerHitClientRPC(float materieanCutoffValue, int currentHP)
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

    public bool PlayerHitServer(ServerRpcParams serverRpcParams = default)
    {
        bool hasPlayrDied = false;
        currentHP--;

        if (currentHP == 0)
        {
            hasPlayrDied = true;
        }

        if (currentHP <= 0)
        {
            GetComponentInParent<PlayerVRShooting>().PlayerDieServer();
            currentHP = 0;
        }
        else
        {
            GetComponentInParent<PlayerVRShooting>().PlayerHit(currentHP);
        }

        float materialCutofValue = 1f - currentHP / (float)maxHP;
        PlayerHitClientRPC(materialCutofValue, currentHP);
        return hasPlayrDied;
    }

    public void RevivePlayerServer()
    {
        currentHP = maxHP;
        RevivePlaterClientRpc();
    }

    [ClientRpc]
    private void RevivePlaterClientRpc()
    {
        mat.SetFloat("_Cutoff", 0f);
        HPText.text = maxHP.ToString();
    }
}
