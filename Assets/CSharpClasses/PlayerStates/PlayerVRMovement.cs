using System.Globalization;
using Unity.Netcode;
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

    private string posKey = "camPos";
    private string rotKey = "camRot";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            LoadExistingCameraCalibration();

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

    private void LoadExistingCameraCalibration()
    {
        Vector3 loadedPos = Vector3.zero;
        loadedPos.x = PlayerPrefs.GetFloat(posKey + "x");
        loadedPos.y = PlayerPrefs.GetFloat(posKey + "y");
        loadedPos.z = PlayerPrefs.GetFloat(posKey + "z");

        Quaternion loadedRot = Quaternion.identity;
        loadedRot.x = PlayerPrefs.GetFloat(rotKey + "x");
        loadedRot.y = PlayerPrefs.GetFloat(rotKey + "y");
        loadedRot.z = PlayerPrefs.GetFloat(rotKey + "z");
        loadedRot.w = PlayerPrefs.GetFloat(rotKey + "w");

        if (loadedPos != Vector3.zero)
            CameraRig.position = loadedPos;
        if (loadedRot != Quaternion.identity)
            CameraRig.rotation = loadedRot;
    }
}
