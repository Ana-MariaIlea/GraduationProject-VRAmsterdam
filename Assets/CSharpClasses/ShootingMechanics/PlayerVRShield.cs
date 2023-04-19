using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

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
            controls = new PlayerInputActions();
            controls.Enable();
            PlayerCreatureHandler.Singleton.part2StartClient.AddListener(BindActions);
        }

        VisualIndication.SetActive(false);
    }

    private void BindActions()
    {
        if (controls != null)
        {
            controls.PlayerPart2.ShootingLeft.performed += ShieldTrigger;
        }
    }

    private void ShieldTrigger(InputAction.CallbackContext ctx)
    {
        if (VisualIndication.activeSelf)
        {
            VisualIndication.SetActive(false);
            //StopCoroutine(MoveShiledCorutine());
        }
        else
        {
            VisualIndication.SetActive(true);
            StartCoroutine(MoveShiledCorutine());
        }
    }
    private IEnumerator MoveShiledCorutine()
    {
        while (VisualIndication.activeSelf)
        {
            VisualIndication.transform.position = anchor.position;
            yield return null;
        }
    }
}
