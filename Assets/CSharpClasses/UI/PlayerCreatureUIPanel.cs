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
    [SerializeField] private Sprite defaultSprite;

    private PlayerInputActions controls;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            controls = new PlayerInputActions();
            controls.Enable();

            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2StartClient);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2StartClient);
                PlayerStateManager.Singleton.part1StartClient.AddListener(BindInputActions);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
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
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.RemoveListener(Part2StartClient);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(Part2StartClient);
                PlayerStateManager.Singleton.part1StartClient.RemoveListener(BindInputActions);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
            controls.Disable();
        }
        base.OnNetworkDespawn();

    }

    void BindInputActions()
    {
        controls.Enable();
        controls.PlayerPart1.OpenCloseInventory.performed += TurnOffOnPanel;
    }

    void UnBindInputActions()
    {
        controls.PlayerPart1.OpenCloseInventory.performed -= TurnOffOnPanel;
    }

    void Part2StartClient()
    {
        UnBindInputActions();
        controls.Disable();
        //TurnOffOnPanelServerRpc(true);
        panel.SetActive(false);
        fireCreatureImage.sprite = defaultSprite;
        waterCreatureImage.sprite = defaultSprite;
        earthCreatureImage.sprite = defaultSprite;
        Part2StartServerRpc();
    }
    [ServerRpc]
    void Part2StartServerRpc()
    {
        panel.SetActive(false);
        //TurnOffOnPanelServerRpc(true);
        fireCreatureImage.sprite = defaultSprite;
        waterCreatureImage.sprite = defaultSprite;
        earthCreatureImage.sprite = defaultSprite;
    }

    private void TurnOffOnPanel(InputAction.CallbackContext ctx)
    {
        TurnOffOnPanelServerRpc();
    }

    [ServerRpc]
    private void TurnOffOnPanelServerRpc(bool isGameModeChanging = false)
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

    public void ColectCreaturServerCall(CreatureType type)
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
        ColectCreatureClientRpc(type);
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
