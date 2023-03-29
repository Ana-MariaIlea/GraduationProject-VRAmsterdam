using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Player movement script used for moving the player based on the
//     position of the headset. The VR rig also gets enabled is the
//     object is a client and the owner
// </summary>
//------------------------------------------------------------------------------
public class PlayerVRMovement : NetworkBehaviour
{
    [SerializeField] private Transform CameraRig;
    [SerializeField] private Transform Head;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            // Enable the camera so that the owning player has control
            CameraRig.gameObject.SetActive(true);
        }
        else
        {
            this.enabled = false;
        }

    }
    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        //Store current 
        Vector3 oldPosition = CameraRig.position;
        Quaternion oldRotation = CameraRig.rotation;

        //Rotate and change position
        transform.eulerAngles = new Vector3(0, Head.rotation.eulerAngles.y, 0);
        transform.position = new Vector3(Head.position.x, 0, Head.position.z);

        //Restore position
        CameraRig.position = oldPosition;
        CameraRig.rotation = oldRotation;
    }
}
