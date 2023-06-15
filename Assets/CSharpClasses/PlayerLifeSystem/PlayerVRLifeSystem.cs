//Made by Ana-Maria Ilea

using TMPro;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Player life system class. This class handles collision with enemy and player projectiles
//     PlayerHit and PlayerDie are called when a collision happens
// </summary>
//------------------------------------------------------------------------------
public class PlayerVRLifeSystem : NetworkBehaviour
{
    [SerializeField] int maxHP = 10;
    [SerializeField] private SoundSource playerHitSoundSource;
    [SerializeField] private SoundSource playerDieSoundSource;


    private int currentHP;
    private bool isPlayerCoOp = true;

    [SerializeField] private GameObject HealthPanel;
    [SerializeField] private TMP_Text HPText;

    [SerializeField] private Material mat;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            if (IsOwner)
            {
                if (PlayerStateManager.Singleton)
                {
                    PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2PlayerVSPlayerStart);
                    PlayerStateManager.Singleton.endingStartClient.AddListener(EndGame);
                }
                else
                {
                    Debug.LogError("No PlayerStateManager in the scene");
                }
            }
        }
    }

    private void EndGame()
    {
        HealthPanel.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer)
        {
            if (IsOwner)
            {
                if (PlayerStateManager.Singleton)
                {
                    PlayerStateManager.Singleton.part2PlayerCoOpStartClient.RemoveListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(Part2Start);
                    PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(Part2PlayerVSPlayerStart);
                    PlayerStateManager.Singleton.endingStartClient.RemoveListener(EndGame);

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
        if (!IsServer) return;
        if (isPlayerCoOp)
        {
            HandlePlayerCoOpTriggers(other);
        }
        else
        {
            HandlePvPTriggers(other);
        }
    }
    private void HandlePlayerCoOpTriggers(Collider other)
    {
        if (other.tag == "EnemyHitObject")
        {
            PlayerHitServer();

            other.GetComponent<EnemyHitObject>().DestroyProjectileServer();
        }
    }

    private void HandlePvPTriggers(Collider other)
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
                    PlayerDiePvPClientRPC();
                }

                other.GetComponent<PlayerHitObject>().DestroyProjectileServer();
            }
        }
    }

    [ClientRpc]
    private void PlayerDiePvPClientRPC()
    {
        PlayerDiePvPServerRpc();
    }

    [ServerRpc]
    private void PlayerDiePvPServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //Server rpc done to get the correct player ID
        ScoreSystemManager.Singleton.DeathAddedToPlayer(serverRpcParams.Receive.SenderClientId);
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
        if (IsOwner)
        {
            PlayerHitServerRpc();

            //Material Cutoff affect the transparency of the health indicator
            mat.SetFloat("_Cutoff", materieanCutoffValue);
            HPText.text = currentHP.ToString();
        }
    }

    [ServerRpc]
    private void PlayerHitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //Server rpc done to get the correct player ID
        if (currentHP == 0)
        {
            SoundManager.Singleton.PlaySoundAllPlayers(playerDieSoundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
        }
        else
        {
            SoundManager.Singleton.PlaySoundAllPlayers(playerHitSoundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
        }
    }

    private void Part2Start()
    {
        isPlayerCoOp = true;
        GetComponent<BoxCollider>().enabled = true;
        HealthPanel.SetActive(true);
        mat.SetFloat("_Cutoff", 1f);
        HPText.text = "0";
        Part2StartServerRpc();
    }

    [ServerRpc]
    private void Part2StartServerRpc()
    {
        isPlayerCoOp = true;
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
