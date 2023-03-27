using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public enum ControllerType
{
    Left,
    Right
}

public class PlayerVRGrabbing : MonoBehaviour
{
    [SerializeField] ControllerType controllerType;
    private PlayerInputActions controls;

    private GrabbableItem grabedItem = null;
    private ItemID grabedItemID = ItemID.None;

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
            return grabedItemID;
        }
        set
        {
            //Some other code
            grabedItemID = value;
        }
    }

    void Awake()
    {
        controls = new PlayerInputActions();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        BindInputActions();
        FindObjectOfType<PlayerStateManager>().part2Start.AddListener(Part2Start);
    }

    private void OnDestroy()
    {
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

            if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID && aux != CreatureType.None)
            {
                Destroy(grabedItem.gameObject);
                grabedItemID = ItemID.None;
                other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleCleared();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabedItem = null;
            grabedItemID = ItemID.None;
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
        this.enabled = false;
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
            if (!GetComponentInParent<PlayerCreatureHandler>().IsFireCretureCollected)
                grabedItemID = grabedItem.IItemID;
        }
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            grabedItem.gameObject.transform.SetParent(null);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = true;
            grabedItemID = ItemID.None;
        }
    }
}
