using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVRShooting : MonoBehaviour
{
    [SerializeField] private Transform controllerLeft;
    [SerializeField] private Transform controllerRight;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector3 projectileOffset;
    [SerializeField] private float projectileShootCooldown = 1;
    [SerializeField] private ParticleSystem streamObjectLeft;
    [SerializeField] private ParticleSystem streamObjectRight;
    [SerializeField] private float streamShootTime = 5;
    [SerializeField] private float streamShootCooldown = 3;
    [SerializeField] private List<ShootingVisualsAndInfo> shootingVisuals;


    private float currentDamage;
    private float currentMaxDamage;

    private PlayerInputActions controls;

    private Coroutine shootingStreamLeft = null;
    private Coroutine shootingStreamRight = null;

    private Coroutine shootingStreamLeftCooldown = null;
    private Coroutine shootingStreamRightCooldown = null;

    private ShootingMode shootingMode = ShootingMode.None;
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

    void Awake()
    {
        controls = new PlayerInputActions();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnDestroy()
    {
        UnBindInputActions();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChargingStation")
        {
            CreatureType aux = other.GetComponent<ChargingStation>().CCreatureType;
            switch (aux)
            {
                case CreatureType.Fire:
                    ChangeShootingModeToProjectile();
                    break;
                case CreatureType.Water:
                    ChangeShootingModeToStream();
                    break;
                case CreatureType.Earth:
                    ChangeShootingModeToProjectile();
                    break;
            }
            for (int i = 0; i < shootingVisuals.Count; i++)
            {
                if (shootingVisuals[i].magicType == aux)
                {
                    currentDamage = shootingVisuals[i].maxDamage;
                    currentMaxDamage = shootingVisuals[i].maxDamage;
                    break;
                }
            }
        }
    }

    private void ChangeShootingModeToStream()
    {
        controls.PlayerPart2.ShootingLeft.performed += ShootStreamLeftProxi;
        controls.PlayerPart2.ShootingRight.performed += ShootStreamRightProxi;

        controls.PlayerPart2.ShootingLeft.canceled += StopShootStreamLeftProxi;
        controls.PlayerPart2.ShootingRight.canceled += StopShootStreamRightProxi;

        if (shootingMode == ShootingMode.Projectile)
        {
            controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
            controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
        }
        shootingMode = ShootingMode.Stream;
    }

    private void ChangeShootingModeToProjectile()
    {
        controls.PlayerPart2.ShootingLeft.performed += ShootProjectileLeftProxi;
        controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
        if (shootingMode == ShootingMode.Stream)
        {
            controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
            controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;

            controls.PlayerPart2.ShootingLeft.canceled -= StopShootStreamLeftProxi;
            controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;
        }
        shootingMode = ShootingMode.Projectile;
    }
    void UnBindInputActions()
    {
        controls.PlayerPart2.ShootingLeft.performed -= ShootProjectileLeftProxi;
        controls.PlayerPart2.ShootingRight.performed -= ShootProjectileRightProxi;
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
        GameObject projectile = Instantiate(projectilePrefab, projectilePosition, projectileRotation);
        projectile.GetComponent<Projectile>().Damage = currentDamage;

        //-------------------------------------------------------------
        //Add player connection;
        //projectile.GetComponent<Projectile>().ShooterPlayerID = 0;
        //-------------------------------------------------------------

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
        if (controller == ControllerType.Left)
        {
            streamObjectLeft.Play();
        }
        else
        {
            streamObjectRight.Play();
        }

        yield return new WaitForSeconds(streamShootTime);

        if (controller == ControllerType.Left)
        {
            streamObjectLeft.Stop();
            controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
            shootingStreamLeft = null;
        }
        else
        {
            streamObjectRight.Stop();
            controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;
            shootingStreamRight = null;
        }
    }

    private IEnumerator ShootSteamPartialCooldown(ControllerType controller)
    {
        if (controller == ControllerType.Left)
        {
            if (shootingStreamLeft != null)
            {
                StopCoroutine(shootingStreamLeft);
                streamObjectLeft.Stop();
                controls.PlayerPart2.ShootingLeft.performed -= ShootStreamLeftProxi;
            }

            controls.PlayerPart2.ShootingLeft.canceled -= StopShootStreamLeftProxi;
        }
        else
        {
            if (shootingStreamRight != null)
            {
                streamObjectRight.Stop();
                StopCoroutine(shootingStreamRight);
                controls.PlayerPart2.ShootingRight.performed -= ShootStreamRightProxi;
            }

            controls.PlayerPart2.ShootingRight.canceled -= StopShootStreamRightProxi;
        }
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
        currentDamage = currentMaxDamage - currentMaxDamage / livesLeft;
        streamObjectLeft.GetComponent<Stream>().Damage = currentDamage;
        streamObjectRight.GetComponent<Stream>().Damage = currentDamage;
    }

    public void PlayerDie()
    {
        switch (shootingMode)
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

        shootingMode = ShootingMode.None;
    }
}
