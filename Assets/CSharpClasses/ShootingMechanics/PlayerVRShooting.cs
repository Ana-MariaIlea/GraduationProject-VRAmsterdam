using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Linq;
using System;

public class PlayerVRShooting : NetworkBehaviour
{
    [SerializeField] private Transform controllerLeft;
    [SerializeField] private Transform controllerRight;

    [SerializeField] private Vector3 projectileOffset;
    [SerializeField] private float projectileShootCooldown = 1;

    [SerializeField] private List<ShootingVisualsAndInfo> shootingVisuals;
    [SerializeField] private SoundSource shootingSoundSource;
    [SerializeField] private SoundSource chargingStationSoundSource;

    private GameObject projectilePrefab;

    private NetworkVariable<float> currentDamage = new NetworkVariable<float>(0);
    private NetworkVariable<float> currentMaxDamage = new NetworkVariable<float>(0);

    private PlayerInputActions controls;

    private bool isPlayerCoOp = true;


    [SerializeField] private NetworkVariable<ShootingMode> shootingMode = new NetworkVariable<ShootingMode>(ShootingMode.None, NetworkVariableReadPermission.Everyone);
    public enum ShootingMode
    {
        None,
        Projectile,
        Stream
    }

    [System.Serializable]
    public struct ShootingVisualsAndInfo
    {
        public CreatureType magicType;
        public float maxDamage;
        public GameObject visualsPrefab;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkSpawn();

            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2PlayerVSPlayerStart);
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2Start);
                PlayerStateManager.Singleton.endingStartClient.AddListener(GameEndClient);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    private void Part2Start()
    {
        controls = new PlayerInputActions();
        controls.Enable();
        isPlayerCoOp = true;
        Part2StartServerRpc();
    }

    [ServerRpc]
    private void Part2StartServerRpc()
    {
        isPlayerCoOp = true;
        StartCoroutine(ChangeShootingModeVariable(CreatureType.None));
    }

    private void GameEndClient()
    {
        switch (shootingMode.Value)
        {
            case ShootingMode.Projectile:
                StopAllCoroutines();
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
                controls.Disable();
                controls = null;
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkDespawn();
            controls.Disable();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(Part2PlayerVSPlayerStart);
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.RemoveListener(Part2Start);
                PlayerStateManager.Singleton.endingStartClient.RemoveListener(GameEndClient);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.tag == "ChargingStation")
            {
                CreatureType aux = other.GetComponent<ChargingStation>().CCreatureType;

                ChargingStationClientRPC();
                ChangeShootingModeToProjectileClientRpc();
                StartCoroutine(ChangeShootingModeVariable(aux));

                for (int i = 0; i < shootingVisuals.Count; i++)
                {
                    if (shootingVisuals[i].magicType == aux)
                    {
                        PlayerReviveServer(i);
                        break;
                    }
                }

                GetComponentInChildren<PlayerVRLifeSystem>().RevivePlayerServer();
            }
        }
    }

    [ClientRpc]
    private void ChargingStationClientRPC()
    {
        ChargingStationServerRpc();
    }

    [ServerRpc]
    private void ChargingStationServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("SoundManager client rpc for charging station");
        SoundManager.Singleton.PlaySoundAllPlayers(chargingStationSoundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
    }
    private IEnumerator ChangeShootingModeVariable(CreatureType aux)
    {
        yield return new WaitForSeconds(.3f);
        switch (aux)
        {
            case CreatureType.Earth:
            case CreatureType.Fire:
            case CreatureType.Water:
                shootingMode.Value = ShootingMode.Projectile;
                break;
            case CreatureType.None:
                shootingMode.Value = ShootingMode.None;
                break;
        }
    }
    [ClientRpc]
    private void ChangeShootingModeToProjectileClientRpc()
    {
        if (controls != null && shootingMode.Value != ShootingMode.Projectile)
        {
            controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
        }
    }

    private void Part2PlayerVSPlayerStart()
    {
        controls = new PlayerInputActions();
        controls.Enable();
        isPlayerCoOp = false;
        Part2PlayerVSPlayerStartServerRpc();
    }

    [ServerRpc]
    private void Part2PlayerVSPlayerStartServerRpc()
    {
        isPlayerCoOp = false;
        StartCoroutine(ChangeShootingModeVariable(CreatureType.None));
    }

    private void ShootProjectileRightProxi(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ShootProjectile(ControllerType.Right));
    }
    private IEnumerator ShootProjectile(ControllerType controller)
    {
        Vector3 projectilePosition = transform.position;
        Quaternion projectileRotation = transform.rotation;

        projectilePosition = controllerRight.position + projectileOffset;
        projectileRotation = controllerRight.rotation;
        controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;

        projectileRotation.z = 0;

        ShootProjectileServerRPC(projectilePosition, projectileRotation.eulerAngles, currentDamage.Value);

        yield return new WaitForSeconds(projectileShootCooldown);

        controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;

    }

    [ServerRpc]
    private void ShootProjectileServerRPC(Vector3 position, Vector3 rotation, float damage, ServerRpcParams serverRpcParams = default)
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.Euler(rotation));
            projectile.GetComponent<Projectile>().Damage = damage;
            projectile.GetComponent<Projectile>().ShooterPlayerID = serverRpcParams.Receive.SenderClientId;
            if (isPlayerCoOp)
            {
                projectile.GetComponent<Projectile>().IsPlayerCoOp = isPlayerCoOp;
                if (gameObject.tag == "Team1")
                {
                    projectile.GetComponent<Projectile>().OpposingTeamTag = "Team2";
                }
                else
                {
                    projectile.GetComponent<Projectile>().OpposingTeamTag = "Team1";
                }
            }
            projectile.GetComponent<NetworkObject>().Spawn();
            SoundManager.Singleton.PlaySoundAllPlayers(shootingSoundSource.SoundID, true, serverRpcParams.Receive.SenderClientId);
        }
        else
        {
            Debug.LogError("ProjectilePrefab is null");
        }
    }

    public void PlayerReviveServer(int projectileDataIndex)
    {
        currentDamage.Value = shootingVisuals[projectileDataIndex].maxDamage;
        currentMaxDamage.Value = shootingVisuals[projectileDataIndex].maxDamage;
        projectilePrefab = shootingVisuals[projectileDataIndex].visualsPrefab;
    }

    public void PlayerHit(int livesLeft)
    {
        currentDamage.Value = currentMaxDamage.Value - currentMaxDamage.Value / livesLeft;
    }

    public void PlayerDieServer()
    {
        PlayerDieClientRpc();
        StartCoroutine(ChangeShootingModeVariable(CreatureType.None));
    }

    [ClientRpc]
    public void PlayerDieClientRpc()
    {
        switch (shootingMode.Value)
        {
            case ShootingMode.Projectile:
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
                break;
        }
    }

    [ServerRpc]
    public void PlayerDieServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ScoreSystemManager.Singleton.DeathAddedToPlayer(serverRpcParams.Receive.SenderClientId);
    }
}
