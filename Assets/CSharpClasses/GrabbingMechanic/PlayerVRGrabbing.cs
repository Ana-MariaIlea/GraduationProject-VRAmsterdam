using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using UnityEngine.XR.ARSubsystems;

public enum ControllerType
{
    Left,
    Right
}

public class PlayerVRGrabbing : NetworkBehaviour
{
    [SerializeField] ControllerType controllerType;
    [SerializeField] private Vector3 grabbingGlobalOffset = new Vector3(0,-1f,0);

    private PlayerInputActions controls;

    private GrabbableItem grabedItem = null;
    private NetworkVariable<ItemID> grabedItemID = new NetworkVariable<ItemID>(ItemID.None);
    private Vector3 grabedItemOffset = Vector3.zero;
    private NetworkVariable<bool> grabbing = new NetworkVariable<bool>(false);
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
            GetComponent<SphereCollider>().enabled = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabedItem = other.GetComponent<GrabbableItem>();
        }
        else if (other.tag == "GrabDestination")
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
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabedItem = null;
            grabedItemID.Value = ItemID.None;
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
        controls.Disable();
        GetComponent<SphereCollider>().enabled = false;
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("try grab");
        if (grabedItem != null)
        {
            Debug.Log("grab-------------------------------");

            GrabItemServerRPC(grabedItem.IItemID);
        }
    }

    [ServerRpc]
    private void GrabItemServerRPC(ItemID id)
    {
        // if (grabedItem != null)
        //{
        //grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
        //grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
        grabbing.Value = true;
        if (!GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
            grabedItemID.Value = id;
        Debug.Log("Server RPC-------------------------------" + grabbing.Value);

        GrabItemClientRPC();

        // }
    }
    //private void Update()
    //{
    //    Debug.Log(OwnerClientId + "  is grabbing " + grabbing.Value + " is server "+ IsServer);
    //}
    [ClientRpc]
    private void GrabItemClientRPC()
    {
        Debug.Log("Client RPC-------------------------------");
        if (grabedItem != null)
        {
            Debug.Log("Client RPCccc-------------------------------  " + grabbing.Value);

            //grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
            grabedItemOffset = transform.position - grabedItem.transform.position;
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(GrabbingObjectCorutine());
            //if (!GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
            //  grabedItemID.Value = grabedItem.IItemID;
        }
    }

    private IEnumerator GrabbingObjectCorutine()
    {
        Debug.Log("start corutine-------------------------------");
        yield return new WaitForEndOfFrame();
        while (grabbing.Value)
        {
            grabedItem.transform.position = transform.position + grabedItemOffset + grabbingGlobalOffset;
            yield return null;
        }
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("ResealseItem-------------------------------");
        ResleaseItemServerRPC();
    }
    [ServerRpc]
    private void ResleaseItemServerRPC()
    {
        //if (grabedItem != null)
        //{
        //grabedItem.gameObject.transform.SetParent(null);
        //grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
        grabbing.Value = false;
        grabedItemID.Value = ItemID.None;
        Debug.Log("Server RPC release-------------------------------" + grabbing.Value);
        ResleaseItemClientRPC();
        // }
    }

    [ClientRpc]
    private void ResleaseItemClientRPC()
    {
        if (grabedItem != null)
        {
            //grabedItem.gameObject.transform.SetParent(null);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            //grabedItemID.Value = ItemID.None;
        }
    }
}
