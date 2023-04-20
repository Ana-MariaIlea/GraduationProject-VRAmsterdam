using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCreatureUIPanel : NetworkBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image fireCreatureImage;
    [SerializeField] private Image waterCreatureImage;
    [SerializeField] private Image earthCreatureImage;

    [SerializeField] private Sprite fireCreatureSprite;
    [SerializeField] private Sprite waterCreatureSprite;
    [SerializeField] private Sprite earthCreatureSprite;

    private PlayerInputActions controls;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            controls = new PlayerInputActions();
            controls.Enable();
            BindInputActions();
            PlayerCreatureHandler.Singleton.part2StartClient.AddListener(Part2Start);
            base.OnNetworkSpawn();
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
            UnBindInputActions();
            controls.Disable();
        }
        base.OnNetworkDespawn();

    }

    void BindInputActions()
    {
        controls.PlayerPart1.OpenCloseInventory.performed += TurnOffOnPanel;
    }

    void UnBindInputActions()
    {
        controls.PlayerPart1.OpenCloseInventory.performed -= TurnOffOnPanel;
    }

    void Part2Start()
    {
        UnBindInputActions();
        controls.Disable();
        TurnOffOnPanelServerRpc(true);
        this.enabled = false;
    }

    private void TurnOffOnPanel(InputAction.CallbackContext ctx)
    {
        TurnOffOnPanelServerRpc();
    }

    [ServerRpc]
    private void TurnOffOnPanelServerRpc(bool isGameModeChanging = false)
    {
        TurnOffOnPanelClientRpc(isGameModeChanging);
    }

    [ClientRpc]
    private void TurnOffOnPanelClientRpc(bool isGameModeChanging = false)
    {
        if (isGameModeChanging)
        {
            panel.SetActive(false);
            return;
        }

        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
    }

    [ClientRpc]
    public void ColectCreatureClientRpc(CreatureType type)
    {
        switch (type)
        {
            case CreatureType.Fire:
                fireCreatureImage.sprite = fireCreatureSprite;
                break;
            case CreatureType.Water:
                waterCreatureImage.sprite = waterCreatureSprite;
                break;
            case CreatureType.Earth:
                earthCreatureImage.sprite = earthCreatureSprite;
                break;
        }
    }
}
