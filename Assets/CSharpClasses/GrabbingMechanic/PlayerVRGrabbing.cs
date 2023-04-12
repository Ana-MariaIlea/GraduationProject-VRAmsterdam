using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using static TMPro.TMP_Compatibility;

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
    [SerializeField] private Vector3 grabbingGlobalOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private Transform anchor;
    private Vector3 anchorPosition = new Vector3(0, -0.5f, 0);

    private PlayerInputActions controls;

    private GrabbableItem grabedItem = null;
    private NetworkVariable<ItemID> grabedItemID = new NetworkVariable<ItemID>(ItemID.None);
    private Vector3 grabedItemOffset = Vector3.zero;
    private NetworkVariable<bool> grabbing = new NetworkVariable<bool>(false);

    private Coroutine updateAnchorPositionCorutine = null;
    //private bool grabbing = false;

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
            BindInputActions();
            PlayerCreatureHandler.Singleton.part2StartClient.AddListener(Part2Start);
            //FindObjectOfType<PlayerStateManager>().part2Start.AddListener(Part2Start);
            base.OnNetworkSpawn();
        }
        else
        {
            //GetComponent<SphereCollider>().enabled = false;
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
        grabedItem = other.GetComponent<GrabbableItem>();
    }
    public void TriggerEnterGrabDestination(Collider other)
    {
        CreatureType otherType = other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType;

        //PlayerCreatureHandler.Singleton.CreatureCollectedServerRpc(otherType);

        if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID.Value &&
            !PlayerCreatureHandler.Singleton.CheckCollectedCreature(otherType, OwnerClientId))
        {
            Debug.Log("Clear obstacle");
            CollectCreatureCallServerRPC(other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType);
            other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleClearedServerRpc();
            DestroyItemServerRPC();
        }
        
    }

    [ServerRpc]
    public void CollectCreatureCallServerRPC(CreatureType creatureType, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Server call " + creatureType);
        PlayerCreatureHandler.Singleton.CreatureCollected(creatureType, serverRpcParams);
    }

    [ServerRpc]
    public void DestroyItemServerRPC()
    {
        Debug.Log("Destroy item");
        StartCoroutine(DestroyItemCorutine());
    }
    public void DestroyItemServerCall()
    {
        //Debug.Log("Destroy item");
        StartCoroutine(DestroyItemCorutine());
    }
    private IEnumerator DestroyItemCorutine()
    {
        GrabbableItem aux = grabedItem;
        ReleaseItemServerCall();
        yield return new WaitForSeconds(.2f);
        GrabbableItemManager.Singleton.RemoveGivenObject(aux);
        aux.GetComponent<NetworkObject>().Despawn();
        Destroy(aux.gameObject);
    }

    public void TriggerExit()
    {
        grabedItem = null;
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
        Debug.Log("Part2start in vr grabbing");
        UnBindInputActions();
        controls.Disable();
        //GetComponent<SphereCollider>().enabled = false;
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            GrabItemServerRPC(grabedItem.IItemID, grabedItem.ObjectID);
        }
    }

    [ServerRpc]
    private void GrabItemServerRPC(ItemID id, int ObjectID)
    {
        grabedItem = GrabbableItemManager.Singleton.FindGivenObject(ObjectID);
        if (grabedItem != null)
        {
            grabedItemID.Value = id;
            grabbing.Value = true;
            //if (GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
            //grabedItemID.Value = ItemID.None;

            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
            GrabItemClientRPC();

            StartCoroutine(GrabbingObjectCorutineServer());
        }
    }

    private IEnumerator GrabbingObjectCorutineServer()
    {
        yield return new WaitForSeconds(.2f);
        grabedItemOffset = anchorPosition - grabedItem.transform.position;

        while (grabbing.Value)
        {
            grabedItem.transform.position = anchorPosition + grabedItemOffset;
            yield return null;
        }
    }

    [ClientRpc]
    private void GrabItemClientRPC()
    {
        if (updateAnchorPositionCorutine == null)
            updateAnchorPositionCorutine = StartCoroutine(GrabbingObjectCorutineClient());
    }
    private IEnumerator GrabbingObjectCorutineClient()
    {
        yield return new WaitForSeconds(.1f);
        while (grabbing.Value)
        {
            SetAnchorServerRPC(anchor.position.x, anchor.position.y - 0.5f, anchor.position.z);
            yield return null;
        }
    }

    [ServerRpc]
    private void SetAnchorServerRPC(float x, float y, float z)
    {
        anchorPosition = new Vector3(x, y, z);
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        ResleaseItemServerRPC();
    }

    public void ReleaseItemServerCall()
    {
        //ResleaseItemServerRPC();
        if (grabedItem != null)
        {
            //Debug.Log("Release item");
            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            ReleaseItemClientRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResleaseItemServerRPC()
    {
        if (grabedItem != null)
        {
            Debug.Log("Release item");
            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            ReleaseItemClientRPC();
        }
    }

    [ClientRpc]
    private void ReleaseItemClientRPC()
    {
        if (updateAnchorPositionCorutine != null)
        {
            StopCoroutine(updateAnchorPositionCorutine);
            updateAnchorPositionCorutine = null;
        }
    }
}
