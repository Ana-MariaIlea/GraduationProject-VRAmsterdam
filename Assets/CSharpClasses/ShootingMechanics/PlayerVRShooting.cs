using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVRShooting : MonoBehaviour
{
    [SerializeField] Transform controllerLeft;
    [SerializeField] Transform controllerRight;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Vector3 projectileOffset;
    [SerializeField] float projectileShootCooldown = 1;
    [SerializeField] ParticleSystem streamObjectLeft;
    [SerializeField] ParticleSystem streamObjectRight;
    [SerializeField] float streamShootTime = 5;
    [SerializeField] float streamShootCooldown = 3;

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
    // Start is called before the first frame update
    void Start()
    {
        //BindInputActions();
    }

    private void OnDestroy()
    {
        UnBindInputActions();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChargingStation")
        {
            switch (other.GetComponent<ChargingStation>().CCreatureType)
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

    void BindInputActions()
    {
        //controls.PlayerPart2.ShootingLeft.performed += ShootProjectileLeftProxi;
        //controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;

        controls.PlayerPart2.ShootingLeft.performed += ShootStreamLeftProxi;
        controls.PlayerPart2.ShootingRight.performed += ShootStreamRightProxi;

        controls.PlayerPart2.ShootingLeft.canceled += StopShootStreamLeftProxi;
        controls.PlayerPart2.ShootingRight.canceled += StopShootStreamRightProxi;
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
        Instantiate(projectilePrefab, projectilePosition, projectileRotation);

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

}
