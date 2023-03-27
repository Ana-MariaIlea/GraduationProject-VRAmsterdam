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
    private PlayerInputActions controls;
    private Coroutine shootingProjectilesLeft = null;
    private Coroutine shootingProjectilesRight = null;
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
        BindInputActions();
    }

    private void OnDestroy()
    {
        UnBindInputActions();
    }

    void BindInputActions()
    {
        controls.PlayerPart2.ShootingLeft.performed += ShootProjectileLeftProxi;
        controls.PlayerPart2.ShootingRight.performed += ShootProjectileRightProxi;
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

}
