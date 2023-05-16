using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Linq;

public class PlayerVRShooting : NetworkBehaviour
{
    [SerializeField] private Transform controllerLeft;
    [SerializeField] private Transform controllerRight;

    [SerializeField] private Vector3 projectileOffset;
    [SerializeField] private float projectileShootCooldown = 1;

    [SerializeField] private ParticleSystem streamObjectLeft;
    [SerializeField] private ParticleSystem streamObjectRight;
    [SerializeField] private float streamShootTime = 5;
    [SerializeField] private float streamShootCooldown = 3;

    [SerializeField] private List<ShootingVisualsAndInfo> shootingVisuals;

    private GameObject projectilePrefab;

    private NetworkVariable<float> currentDamage = new NetworkVariable<float>(0);
    private NetworkVariable<float> currentMaxDamage = new NetworkVariable<float>(0);

    private PlayerInputActions controls;

    private Coroutine shootingStreamLeft = null;
    private Coroutine shootingStreamRight = null;

    private Coroutine shootingStreamLeftCooldown = null;
    private Coroutine shootingStreamRightCooldown = null;

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
            controls = new PlayerInputActions();
            controls.Enable();

            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2StartClient.AddListener(Part2Start);
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

    public override void OnNetworkDespawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkDespawn();
            //PlayerDieClientRpc();
            controls.Disable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.tag == "ChargingStation")
            {
                CreatureType aux = other.GetComponent<ChargingStation>().CCreatureType;
                switch (aux)
                {
                    case CreatureType.Earth:
                    case CreatureType.Fire:
                        ChangeShootingModeToProjectileClientRpc();
                        break;
                    case CreatureType.Water:
                        ChangeShootingModeToStreamClientRpc();
                        break;
                }
                StartCoroutine(ChangeShootingModeVariable(aux));

                for (int i = 0; i < shootingVisuals.Count; i++)
                {
                    if (shootingVisuals[i].magicType == aux)
                    {
                        PlayerReviveServerRpc(shootingVisuals[i].maxDamage);
                        projectilePrefab = shootingVisuals[i].visualsPrefab;
                        break;
                    }
                }
            }
        }
    }
    private IEnumerator ChangeShootingModeVariable(CreatureType aux)
    {
        yield return new WaitForSeconds(.3f);
        switch (aux)
        {
            case CreatureType.Earth:
            case CreatureType.Fire:
                shootingMode.Value = ShootingMode.Projectile;
                break;
            case CreatureType.Water:
                shootingMode.Value = ShootingMode.Stream;
                break;
        }
    }
    [ClientRpc]
    private void ChangeShootingModeToStreamClientRpc()
    {
        if (controls != null && shootingMode.Value != ShootingMode.Stream)
        {
            controls.PlayerPart2.ShootingRight.performed += ShootStreamRightProxi;

            controls.PlayerPart2.ShootingRight.canceled += StopShootStreamRightProxi;

            if (shootingMode.Value == ShootingMode.Projectile)
            {
                Debug.Log("disable projectile");
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
            }
        }
    }
    [ClientRpc]
    private void ChangeShootingModeToProjectileClientRpc()
    {
        if (controls != null && shootingMode.Value != ShootingMode.Projectile)
        {
            controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
            if (shootingMode.Value == ShootingMode.Stream)
            {
                Debug.Log("disable stream");

                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;

                controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;
            }
        }
    }

    private void Part2Start()
    {

    }

    private void ShootProjectileLeftProxi(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ShootProjectile(ControllerType.Left));
    }
    private void ShootProjectileRightProxi(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ShootProjectile(ControllerType.Right));
    }
    private IEnumerator ShootProjectile(ControllerType controller)
    {
        Vector3 projectilePosition = transform.position;
        Quaternion projectileRotation = transform.rotation;
        if (controller == ControllerType.Left)
        {
            projectilePosition = controllerLeft.position + projectileOffset;
            projectileRotation = controllerLeft.rotation;
            controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
        }
        else
        {
            projectilePosition = controllerRight.position + projectileOffset;
            projectileRotation = controllerRight.rotation;
            controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
        }
        projectileRotation.z = 0;

        ShootProjectileServerRPC(projectilePosition.x, projectilePosition.y, projectilePosition.z,
                                 projectileRotation.eulerAngles.x, projectileRotation.eulerAngles.y, projectileRotation.eulerAngles.z,
                                 currentDamage.Value);

        yield return new WaitForSeconds(projectileShootCooldown);

        if (controller == ControllerType.Left)
        {
            controls.PlayerPart2.ShootingLeft.performed += ShootProjectileLeftProxi;
        }
        else
        {
            controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
        }
    }

    [ServerRpc]
    private void ShootProjectileServerRPC(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float damage, ServerRpcParams serverRpcParams = default)
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, new Vector3(posX, posY, posZ), Quaternion.Euler(rotX, rotY, rotZ));
            projectile.GetComponent<Projectile>().Damage = damage;
            projectile.GetComponent<Projectile>().ShooterPlayerID = serverRpcParams.Receive.SenderClientId;
            projectile.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            Debug.LogError("ProjectilePrefab is null");
        }
    }

    private void ShootStreamRightProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamRight == null)
        {
            Debug.Log("Start Stream----------------------");
            shootingStreamRight = StartCoroutine(ShootSteam());
        }
    }

    private void StopShootStreamRightProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamRight != null)
        {
            StopCoroutine(shootingStreamRight);
            StopStreamServerRPC();
            shootingStreamRight = null;
        }
    }

    private IEnumerator ShootSteam()
    {
        ShootStreamServerRPC();

        StartCoroutine(UpdateRightStreamCorutineClient());

        yield return new WaitForSeconds(streamShootTime);

        StopStreamServerRPC();

        shootingStreamRight = null;
    }
    private IEnumerator UpdateRightStreamCorutineClient()
    {
        yield return new WaitForSeconds(.3f);
        while (streamObjectRight.isPlaying)
        {
            UpdateRightStreamPositionServerRPC(controllerRight.position, controllerRight.rotation.eulerAngles);
            yield return null;
        }
    }

    [ServerRpc]
    private void UpdateRightStreamPositionServerRPC(Vector3 pos, Vector3 rot)
    {
        streamObjectRight.transform.position = pos;
        streamObjectRight.transform.rotation = Quaternion.Euler(rot);
    }

    [ServerRpc]
    private void ShootStreamServerRPC()
    {
        streamObjectRight.Play();
        
        ShootStreamClientRPC();
    }

    [ClientRpc]
    private void ShootStreamClientRPC()
    {
        streamObjectRight.Play();
    }

    [ServerRpc]
    private void StopStreamServerRPC()
    {
        if (streamObjectRight.isPlaying)
            streamObjectRight.Stop();
        StopStreamClientRPC();
    }

    [ClientRpc]
    private void StopStreamClientRPC()
    {
        if (streamObjectRight.isPlaying)
            streamObjectRight.Stop();
    }



    [ServerRpc]
    public void PlayerReviveServerRpc(float maxDamage)
    {
        currentDamage.Value = maxDamage;
        currentMaxDamage.Value = maxDamage;
        streamObjectLeft.GetComponent<Stream>().Damage = maxDamage;
        streamObjectRight.GetComponent<Stream>().Damage = maxDamage;
    }

    public void PlayerHit(int livesLeft)
    {
        currentDamage.Value = currentMaxDamage.Value - currentMaxDamage.Value / livesLeft;
        streamObjectLeft.GetComponent<Stream>().Damage = currentDamage.Value;
        streamObjectRight.GetComponent<Stream>().Damage = currentDamage.Value;
    }

    [ClientRpc]
    public void PlayerDieClientRpc(ClientRpcParams clientRpcParams)
    {
        switch (shootingMode.Value)
        {
            case ShootingMode.Projectile:
                controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
                break;
            case ShootingMode.Stream:
                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;

                controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;

                StopAllCoroutines();
                shootingStreamLeft = null;
                shootingStreamRight = null;
                shootingStreamLeftCooldown = null;
                shootingStreamRightCooldown = null;
                break;
        }

        //shootingMode = ShootingMode.None;
    }
}
