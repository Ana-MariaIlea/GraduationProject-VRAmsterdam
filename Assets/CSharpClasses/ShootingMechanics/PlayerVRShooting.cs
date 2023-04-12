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

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector3 projectileOffset;
    [SerializeField] private float projectileShootCooldown = 1;

    [SerializeField] private GameObject StreamPrefab;

    [SerializeField] private ParticleSystem streamObjectLeft;
    [SerializeField] private ParticleSystem streamObjectRight;
    [SerializeField] private float streamShootTime = 5;
    [SerializeField] private float streamShootCooldown = 3;

    [SerializeField] private List<ShootingVisualsAndInfo> shootingVisuals;

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
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkDespawn();
            PlayerDie();
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
                        currentDamage.Value = shootingVisuals[i].maxDamage;
                        currentMaxDamage.Value = shootingVisuals[i].maxDamage;
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
            controls.PlayerPart2.ShootingLeft.performed += ShootStreamLeftProxi;
            controls.PlayerPart2.ShootingRight.performed += ShootStreamRightProxi;

            controls.PlayerPart2.ShootingLeft.canceled += StopShootStreamLeftProxi;
            controls.PlayerPart2.ShootingRight.canceled += StopShootStreamRightProxi;

            if (shootingMode.Value == ShootingMode.Projectile)
            {
                Debug.Log("disable projectile");
                controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
            }
        }
    }
    [ClientRpc]
    private void ChangeShootingModeToProjectileClientRpc()
    {
        if (controls != null && shootingMode.Value != ShootingMode.Projectile)
        {
            controls.PlayerPart2.ShootingLeft.performed += ShootProjectileLeftProxi;
            controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
            if (shootingMode.Value == ShootingMode.Stream)
            {
                Debug.Log("disable stream");

                controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;

                controls.PlayerPart2.ShootingLeft.canceled -= StopShootStreamLeftProxi;
                controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;
            }
        }
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
    private void ShootProjectileServerRPC(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float damage)
    {
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(posX, posY, posZ), Quaternion.Euler(rotX, rotY, rotZ));
        projectile.GetComponent<Projectile>().Damage = damage;
        projectile.GetComponent<NetworkObject>().Spawn();
    }

    private void ShootStreamLeftProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamLeft == null)
        {
            shootingStreamLeft = StartCoroutine(ShootSteam(ControllerType.Left));
        }
    }
    private void ShootStreamRightProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamRight == null)
        {
            shootingStreamRight = StartCoroutine(ShootSteam(ControllerType.Right));
        }
    }

    private void StopShootStreamLeftProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamLeftCooldown == null)
        {
            shootingStreamLeftCooldown = StartCoroutine(ShootSteamPartialCooldown(ControllerType.Left));
        }
    }
    private void StopShootStreamRightProxi(InputAction.CallbackContext ctx)
    {
        if (shootingStreamRightCooldown == null)
        {
            shootingStreamRightCooldown = StartCoroutine(ShootSteamPartialCooldown(ControllerType.Right));
        }
    }

    private IEnumerator ShootSteam(ControllerType controller)
    {
        ShootStreamServerRPC(controller);

        if (controller == ControllerType.Left)
        {
            StartCoroutine(UpdateLeftStreamCorutineClient());
        }
        else
        {
            StartCoroutine(UpdateRightStreamCorutineClient());
        }

        yield return new WaitForSeconds(streamShootTime);

        StopStreamServerRPC(controller);

        if (controller == ControllerType.Left)
        {
            controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
            shootingStreamLeft = null;
        }
        else
        {
            controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;
            shootingStreamRight = null;
        }
    }
    [ServerRpc]
    private void ShootStreamServerRPC(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            streamObjectLeft.Play();
        }
        else
        {
            streamObjectRight.Play();
        }
        ShootStreamClientRPC(controller);
    }
    [ClientRpc]
    private void ShootStreamClientRPC(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            streamObjectLeft.Play();
        }
        else
        {
            streamObjectRight.Play();
        }
    }

    [ServerRpc]
    private void StopStreamServerRPC(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            if (streamObjectLeft.isPlaying)
                streamObjectLeft.Stop();
        }
        else
        {
            if (streamObjectRight.isPlaying)
                streamObjectRight.Stop();
        }
        StopStreamClientRPC(controller);
    }

    [ClientRpc]
    private void StopStreamClientRPC(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            if (streamObjectLeft.isPlaying)
                streamObjectLeft.Stop();
        }
        else
        {
            if (streamObjectRight.isPlaying)
                streamObjectRight.Stop();
        }
    }

    [ServerRpc]
    private void UpdateLeftStreamPositionServerRPC(float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        streamObjectLeft.transform.localPosition = new Vector3(posX, posY, posZ);
        streamObjectLeft.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    [ServerRpc]
    private void UpdateRightStreamPositionServerRPC(float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        streamObjectRight.transform.localPosition = new Vector3(posX, posY, posZ);
        streamObjectRight.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
    }
    private IEnumerator UpdateLeftStreamCorutineClient()
    {
        yield return new WaitForSeconds(.3f);
        while (streamObjectLeft.isPlaying)
        {
            UpdateLeftStreamPositionServerRPC(controllerLeft.position.x, controllerLeft.position.y, controllerLeft.position.z,
                controllerLeft.rotation.eulerAngles.x, controllerLeft.rotation.eulerAngles.y, controllerLeft.rotation.eulerAngles.z);
            yield return null;
        }
    }
    private IEnumerator UpdateRightStreamCorutineClient()
    {
        yield return new WaitForSeconds(.3f);
        while (streamObjectRight.isPlaying)
        {
            UpdateRightStreamPositionServerRPC(controllerRight.position.x, controllerRight.position.y, controllerRight.position.z,
                controllerRight.rotation.eulerAngles.x, controllerRight.rotation.eulerAngles.y, controllerRight.rotation.eulerAngles.z);//,
                                                                                                                                        //clientID.Value);
            yield return null;
        }
    }



    private IEnumerator ShootSteamPartialCooldown(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            if (shootingStreamLeft != null)
            {
                StopCoroutine(shootingStreamLeft);
                controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
            }

            controls.PlayerPart2.ShootingLeft.canceled -= StopShootStreamLeftProxi;
        }
        else
        {
            if (shootingStreamRight != null)
            {
                StopCoroutine(shootingStreamRight);
                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;
            }

            controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;
        }
        StopStreamServerRPC(controller);
        // add formula for partial stream cooldown
        yield return new WaitForSeconds(streamShootCooldown);

        if (controller == ControllerType.Left)
        {
            shootingStreamLeft = null;
            shootingStreamLeftCooldown = null;

            controls.PlayerPart2.ShootingLeft.performed += ShootStreamLeftProxi;

            controls.PlayerPart2.ShootingLeft.canceled += StopShootStreamLeftProxi;
        }
        else
        {
            shootingStreamRight = null;
            shootingStreamRightCooldown = null;

            controls.PlayerPart2.ShootingRight.performed += ShootStreamRightProxi;

            controls.PlayerPart2.ShootingRight.canceled += StopShootStreamRightProxi;
        }
    }

    public void PlayerHit(int livesLeft)
    {
        currentDamage.Value = currentMaxDamage.Value - currentMaxDamage.Value / livesLeft;
        streamObjectLeft.GetComponent<Stream>().Damage = currentDamage.Value;
        streamObjectRight.GetComponent<Stream>().Damage = currentDamage.Value;
    }

    public void PlayerDie()
    {
        switch (shootingMode.Value)
        {
            case ShootingMode.Projectile:
                controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
                controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
                break;
            case ShootingMode.Stream:
                controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;

                controls.PlayerPart2.ShootingLeft.canceled -= StopShootStreamLeftProxi;
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
