using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControllerType
{
    Left,
    Right
}

//------------------------------------------------------------------------------
// </summary>
//     VR Grabbing script attacted to both controllers. Used in Part 1 to grab items
// </summary>
//------------------------------------------------------------------------------
public class PlayerVRGrabbing : NetworkBehaviour
{
    [SerializeField] ControllerType controllerType;

    [SerializeField] private Transform anchor;
    private NetworkVariable<Vector3> anchorPositionNetworked = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] PlayerCreatureUIPanel creatureUIPanel;

    private PlayerInputActions controls;

    private GrabbableItem grabedItem = null;
    private NetworkVariable<ItemID> grabedItemID = new NetworkVariable<ItemID>(ItemID.None);
    private Vector3 grabedItemOffset = Vector3.zero;
    private NetworkVariable<bool> grabbing = new NetworkVariable<bool>(false);

    public GrabbableItem GrabedItem
    {
        get
        {
            //Some other code
            return grabedItem;
        }
    }

    public ItemID GrabedItemID
    {
        get
        {
            //Some other code
            return grabedItemID.Value;
        }
        set
        {
            //Some other code
            grabedItemID.Value = value;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            controls = new PlayerInputActions();
            controls.Enable();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part1StartClient.AddListener(BindInputActions);
                PlayerStateManager.Singleton.part2StartClient.AddListener(Part2Start);
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
            controls.Disable();
        }
        base.OnNetworkDespawn();

    }
    public void TriggerEnterGrab(Collider other)
    {
        if (grabbing.Value == false)
        {
            grabedItem = other.GetComponent<GrabbableItem>();
        }
    }
    public void TriggerEnterGrabDestination(Collider other)
    {
        CreatureType otherType = other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType;

        if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID.Value &&
            !PlayerCreatureHandler.Singleton.CheckCollectedCreature(otherType, OwnerClientId))
        {
            Debug.Log("Clear obstacle");
            CollectCreatureCallServerRpc(other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType);
            other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleClearedServerRpc();
            DestroyItemServerRpc();
        }

    }

    [ServerRpc]
    public void CollectCreatureCallServerRpc(CreatureType creatureType, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Server call " + creatureType);
        PlayerCreatureHandler.Singleton.CreatureCollected(creatureType, serverRpcParams);
        creatureUIPanel.ColectCreaturServerCall(creatureType);
    }

    [ServerRpc]
    public void DestroyItemServerRpc()
    {
        Debug.Log("Destroy item");
        StartCoroutine(DestroyItemCorutine());
    }
    public void DestroyItemServerCall()
    {
        StartCoroutine(DestroyItemCorutine());
    }
    private IEnumerator DestroyItemCorutine()
    {
        GrabbableItem aux = grabedItem;
        ReleaseItemServerCall();
        yield return new WaitForFixedUpdate();
        GrabbableItemManager.Singleton.RemoveGivenObject(aux);
        aux.GetComponent<NetworkObject>().Despawn();
        Destroy(aux.gameObject);
    }

    public void TriggerExit()
    {
        if (grabbing.Value == false) 
        {
            grabedItem = null;
        }
    }

    void BindInputActions()
    {
        if (controllerType == ControllerType.Left)
        {
            controls.PlayerPart1.GrabbingLeft.performed += GrabItem;
            controls.PlayerPart1.GrabbingLeft.canceled += ResealseItem;
        }
        else
        {
            controls.PlayerPart1.GrabbingRight.performed += GrabItem;
            controls.PlayerPart1.GrabbingRight.canceled += ResealseItem;
        }

        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.part1StartClient.RemoveListener(BindInputActions);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }

    }

    void UnBindInputActions()
    {
        if (controllerType == ControllerType.Left)
        {
            controls.PlayerPart1.GrabbingLeft.performed -= GrabItem;
            controls.PlayerPart1.GrabbingLeft.canceled -= ResealseItem;
        }
        else
        {
            controls.PlayerPart1.GrabbingRight.performed -= GrabItem;
            controls.PlayerPart1.GrabbingRight.canceled -= ResealseItem;
        }
    }

    private void Update()
    {
        if (!IsServer)
            anchorPositionNetworked.Value = anchor.position;
    }
    void Part2Start()
    {
        Debug.Log("Part2start in vr grabbing");
        UnBindInputActions();

        PlayerStateManager.Singleton.part2StartClient.RemoveListener(Part2Start);

        controls.Disable();
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        Debug.Log(grabedItem);
        if (grabedItem != null)
        {
            GrabItemServerRpc(grabedItem.ObjectID);
        }
    }

    [ServerRpc]
    private void GrabItemServerRpc(int ObjectID, ServerRpcParams serverRpcParams = default)
    {
        grabedItem = GrabbableItemManager.Singleton.FindGivenObject(ObjectID);
        if (grabedItem != null)
        {
            grabedItemID.Value = grabedItem.IItemID;
            grabbing.Value = true;

            grabedItem.GetComponent<NetworkObject>().ChangeOwnership(serverRpcParams.Receive.SenderClientId);
            GrabClientRpc();
        }
    }

    [ClientRpc]
    private void GrabClientRpc()
    {
        StartCoroutine(GrabbingObjectCorutineServer());
    }

    private IEnumerator GrabbingObjectCorutineServer()
    {

        yield return new WaitForFixedUpdate();
        grabedItemOffset = grabedItem.transform.position - anchor.position;

        while (grabbing.Value && grabedItem != null)
        {
            grabedItem.transform.position = anchor.position + grabedItemOffset;
            yield return null;
        }
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            ResleaseItemServerRpc();
        }

    }

    public void ReleaseItemServerCall()
    {
        if (grabedItem != null)
        {
            grabedItem.GetComponent<NetworkObject>().RemoveOwnership();
            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
        }
    }

    [ServerRpc]
    private void ResleaseItemServerRpc()
    {
        if (grabedItem != null)
        {
            grabedItem.GetComponent<NetworkObject>().RemoveOwnership();
            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
        }
    }
}
