//Made by Ana-Maria Ilea

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
            return grabedItem;
        }
    }

    public ItemID GrabedItemID
    {
        get
        {
            return grabedItemID.Value;
        }
        set
        {
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
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.AddListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.AddListener(Part2Start);
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
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part1StartClient.RemoveListener(BindInputActions);
                PlayerStateManager.Singleton.part2PlayerCoOpStartClient.RemoveListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartClient.RemoveListener(Part2Start);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
        base.OnNetworkDespawn();

    }
    public void TriggerEnterGrab(Collider other)
    {
        //Function called by the VRGrabbingColisionHandler when the controllers collides with an item
        if (grabbing.Value == false)
        {
            grabedItem = other.GetComponent<GrabbableItem>();
        }
    }

    public void TriggerExit()
    {
        //Function called by the VRGrabbingColisionHandler when the controllers ends collision with an item
        if (grabbing.Value == false)
        {
            grabedItem = null;
        }
    }

    public void TriggerEnterGrabDestination(Collider other)
    {
        //Function called by the VRGrabbingColisionHandler when the controllers collides with an obstacle
        CreatureType otherType = other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType;

        if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID.Value &&
            !PlayerCreatureHandler.Singleton.CheckCollectedCreature(otherType, OwnerClientId) &&
            other.GetComponent<FriendlyCreatureItemObstacle>().isObstacleClear == false)
        {
            //If the player has the correct item, has not colected the creature type and the obstacle is not cleared
            CollectCreatureCallServerRPC(other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType);
            other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleClearedServerRpc();
            DestroyItemServerRPC();
        }

    }

    [ServerRpc]
    public void CollectCreatureCallServerRPC(CreatureType creatureType, ServerRpcParams serverRpcParams = default)
    {
        PlayerCreatureHandler.Singleton.CreatureCollected(creatureType, serverRpcParams);
        creatureUIPanel.ColectCreaturServerCall(creatureType);
    }

    [ServerRpc]
    public void DestroyItemServerRPC()
    {
        StartCoroutine(DestroyItemCorutine());
    }

    private IEnumerator DestroyItemCorutine()
    {
        if (grabedItem != null)
        {
            GrabbableItem aux = grabedItem;
            ReleaseItemServerCall();
            yield return new WaitForFixedUpdate();
            GrabbableItemManager.Singleton.RemoveGivenObject(aux);
            aux.GetComponent<NetworkObject>().Despawn();
            Destroy(aux.gameObject);
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

    void Part2Start()
    {
        UnBindInputActions();
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            GrabItemServerRPC(grabedItem.ObjectID);
        }
    }

    [ServerRpc]
    private void GrabItemServerRPC(int ObjectID, ServerRpcParams serverRpcParams = default)
    {
        //Find the objects the player is grabbing
        grabedItem = GrabbableItemManager.Singleton.FindGivenObject(ObjectID);
        
        if (grabedItem != null)
        {
            grabedItemID.Value = grabedItem.IItemID;
            grabbing.Value = true;

            //Change ownership so that the player can change the position of the item
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
        //Wait for the networking variables to update
        yield return new WaitForSeconds(0.1f);
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
            ResleaseItemServerRPC();
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
    private void ResleaseItemServerRPC()
    {
        if (grabedItem != null)
        {
            grabedItem.GetComponent<NetworkObject>().RemoveOwnership();
            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
        }
    }
}
