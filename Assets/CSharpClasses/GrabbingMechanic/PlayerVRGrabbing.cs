using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class PlayerVRGrabbing : MonoBehaviour
{
    public enum ControllerType
    {
        Left,
        Right
    }

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
            if (other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleItemID == grabedItemID)
            {
                Destroy(grabedItem.gameObject);
                grabedItemID = ItemID.None;
                other.GetComponent<FriendlyCreatureItemObstacle>().ObstacleCleared();
                other.GetComponent<FriendlyCreatureItemObstacle>().GetComponent<BoxCollider>().enabled = false;
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

    // Update is called once per frame
    void Update()
    {

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

    void GrabItem(InputAction.CallbackContext ctx)
    {
        if (grabedItem != null)
        {
            grabedItem.gameObject.transform.SetParent(this.gameObject.transform);
            grabedItem.gameObject.GetComponent<SphereCollider>().enabled = false;
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
