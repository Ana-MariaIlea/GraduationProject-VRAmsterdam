using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
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
        if (IsOwner)
        {
            controls = new PlayerInputActions();
            controls.Enable();
            BindInputActions();
            //FindObjectOfType<PlayerStateManager>().part2Start.AddListener(Part2Start);
        }
        else
        {
            //GetComponent<SphereCollider>().enabled = false;
            this.enabled = false;
        }
        base.OnNetworkSpawn();

    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
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
        CreatureType aux = CreatureType.None;
        switch (other.GetComponent<FriendlyCreatureItemObstacle>().CCreatureType)
        {
            case CreatureType.Water:
                if (GetComponentInParent<PlayerCreatureHandler>().IsWaterCretureCollected) return;
                aux = CreatureType.Water;
                break;
            case CreatureType.Earth:
                if (GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected) return;
                aux = CreatureType.Earth;
                break;
        }

        if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID.Value && aux != CreatureType.None)
        {
            Destroy(grabedItem.gameObject);
            //grabedItem.GetComponent<NetworkObject>().Despawn(true);
            grabedItemID.Value = ItemID.None;
            other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleCleared();
        }
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
        UnBindInputActions();
        controls.Disable();
        //GetComponent<SphereCollider>().enabled = false;
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("try grab");
        if (grabedItem != null)
        {
            Debug.Log("grab-------------------------------");

            GrabItemServerRPC(grabedItem.IItemID, grabedItem.ObjectID);
        }
    }

    [ServerRpc]
    private void GrabItemServerRPC(ItemID id, int ObjectID)
    {
        grabedItem = GrabbableItemManager.Singleton.FindGivenObject(ObjectID);
        if (grabedItem != null)
        {
            Debug.Log("Server RPC-------------------------------" + grabbing.Value);
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
        Debug.Log("start corutine Server-------------------------------");
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.2f);
        grabedItemOffset = anchorPosition - grabedItem.transform.position;

        while (grabbing.Value)
        {
            grabedItem.transform.position = anchorPosition + grabedItemOffset;// + grabbingGlobalOffset;
            yield return null;
        }
    }

    [ClientRpc]
    private void GrabItemClientRPC()
    {
        Debug.Log("Client RPC-------------------------------");
        if (updateAnchorPositionCorutine == null)
            updateAnchorPositionCorutine = StartCoroutine(GrabbingObjectCorutineClient());
    }
    private IEnumerator GrabbingObjectCorutineClient()
    {
        Debug.Log("start corutine-------------------------------");
        yield return new WaitForSeconds(.1f);
        while (grabbing.Value)
        {
            Debug.Log(anchor.position);
            SetAnchorServerRPC(anchor.position.x, anchor.position.y -0.5f, anchor.position.z);
            yield return null;
        }
    }

    [ServerRpc]
    private void SetAnchorServerRPC(float x, float y, float z)
    {
        Debug.Log("Change anchor server RPC");
        anchorPosition = new Vector3(x, y, z);// + grabbingGlobalOffset;
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("ResealseItem-------------------------------");
        ResleaseItemServerRPC();
    }
    [ServerRpc]
    private void ResleaseItemServerRPC()
    {
        if (grabedItem != null)
        {
            Debug.Log("Server RPC release-------------------------------" + grabbing.Value);

            grabbing.Value = false;
            grabedItemID.Value = ItemID.None;
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            ReleaseItemClientRPC();
        }
    }

    [ClientRpc]
    private void ReleaseItemClientRPC()
    {
        Debug.Log("Client RPC-------------------------------");
        if (updateAnchorPositionCorutine != null)
        {
            StopCoroutine(updateAnchorPositionCorutine);
            updateAnchorPositionCorutine = null;
        }
    }
}
