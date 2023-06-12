using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class PlayerVRShield : NetworkBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private GameObject VisualIndication;

    private PlayerInputActions controls;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkSpawn();
            
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(BindActions);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(BindActions);
                PlayerStateManager.Singleton.endingStartClient.AddListener(EndGame);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }

        VisualIndication.SetActive(false);
    }

    private void EndGame()
    {
        if (controls != null)
        {
            controls.PlayerPart2.ShootingLeft.performed += ShieldTrigger;
            controls.Disable();
            controls = null;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && IsClient)
        {
            base.OnNetworkDespawn();
            if (controls != null)
            {
                controls.PlayerPart2.ShootingLeft.performed -= ShieldTrigger;
                controls.Disable();
            }
            

            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.RemoveListener(BindActions);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(BindActions);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }

        VisualIndication.SetActive(false);
    }

    private void BindActions()
    {
        controls = new PlayerInputActions();
        controls.Enable();
        if (controls != null)
        {
            controls.PlayerPart2.ShootingLeft.performed += ShieldTrigger;
        }
    }

    private void ShieldTrigger(InputAction.CallbackContext ctx)
    {
        if (VisualIndication.activeSelf)
        {
            SetShieldVisibilityServerRpc(false);
            VisualIndication.SetActive(false);
        }
        else
        {
            SetShieldVisibilityServerRpc(true);
            VisualIndication.SetActive(true);
            StartCoroutine(MoveShiledCorutine());
        }
    }
    private IEnumerator MoveShiledCorutine()
    {
        while (VisualIndication.activeSelf)
        {
            VisualIndication.transform.position = anchor.position;
            MoveShieldServerRpc(VisualIndication.transform.position);
            yield return null;
        }
    }

    [ServerRpc]
    private void SetShieldVisibilityServerRpc(bool isActive)
    {
        VisualIndication.SetActive(isActive);
        SetShieldVisibilityClientRpc(isActive);
    }

    [ClientRpc]
    private void SetShieldVisibilityClientRpc(bool isActive)
    {
        VisualIndication.SetActive(isActive);
    }

    [ServerRpc]
    private void MoveShieldServerRpc(Vector3 position)
    {
        VisualIndication.transform.position = position;
        MoveShieldClientRpc(position);
    }

    [ClientRpc]
    private void MoveShieldClientRpc(Vector3 position)
    {
        VisualIndication.transform.position = position;
    }
}
