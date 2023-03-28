using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public enum ControllerType
{
    Left,
    Right
}

public class PlayerVRGrabbing : NetworkBehaviour
{
    [SerializeField] ControllerType controllerType;
    private PlayerInputActions controls;

    private GrabbableItem grabedItem = null;
    private NetworkVariable<ItemID> grabedItemID = new NetworkVariable<ItemID>(ItemID.None);

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

    private void OnDisable()
    {
        controls.Disable();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            controls = new PlayerInputActions();
            controls.Enable();
            BindInputActions();
            FindObjectOfType<PlayerStateManager>().part2Start.AddListener(Part2Start);
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;
            this.enabled = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
            UnBindInputActions();
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
        GetComponent<SphereCollider>().enabled = false;
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        GrabItemServerRPC();
    }

    [ServerRpc]
    private void GrabItemServerRPC()
    {
        if (grabedItem != null)
        {
            //grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
            //grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
            GrabItemClientRPC();
            if (!GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
                grabedItemID.Value = grabedItem.IItemID;
        }
    }

    [ClientRpc]
    private void GrabItemClientRPC()
    {
        if (grabedItem != null)
        {
            grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
            //if (!GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
              //  grabedItemID.Value = grabedItem.IItemID;
        }
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        ResleaseItemServerRPC();
    }
    [ServerRpc]
    private void ResleaseItemServerRPC()
    {
        //if (grabedItem != null)
        //{
            //grabedItem.gameObject.transform.SetParent(null);
            //grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            grabedItemID.Value = ItemID.None;
        ResleaseItemClientRPC();
       // }
    }

    [ClientRpc]
    private void ResleaseItemClientRPC()
    {
        if (grabedItem != null)
        {
            grabedItem.gameObject.transform.SetParent(null);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            //grabedItemID.Value = ItemID.None;
        }
    }
}
