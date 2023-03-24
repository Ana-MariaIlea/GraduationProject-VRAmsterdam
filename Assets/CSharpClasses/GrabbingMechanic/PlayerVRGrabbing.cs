using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVRGrabbing : MonoBehaviour
{
    public enum ControllerType
    {
        Left,
        Right
    }

    [SerializeField] ControllerType controllerType;
    private PlayerInputActions controls;

    private GrabbableItem grabbable = null;

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
            grabbable = other.GetComponent<GrabbableItem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grab")
        {
            grabbable = null;
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
            controls.PlayerPart1.GrabbingLeft.performed += GrabItem;
            controls.PlayerPart1.GrabbingLeft.canceled += ResealseItem;
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
            controls.PlayerPart1.GrabbingLeft.performed -= GrabItem;
            controls.PlayerPart1.GrabbingLeft.canceled -= ResealseItem;
        }
    }

    void GrabItem(InputAction.CallbackContext ctx)
    {
        if(grabbable != null)
        {
            grabbable.gameObject.transform.SetParent(this.gameObject.transform);
        }
    }

    void ResealseItem(InputAction.CallbackContext ctx)
    {
        if (grabbable != null)
        {
            grabbable.gameObject.transform.SetParent(null);
            grabbable = null;
        }
    }
}
