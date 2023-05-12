using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnObjectOnControllerPressed : NetworkBehaviour
{
    public Transform rightAnchor;
    public GameObject testObjectPrefab;

    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                SpawnObjectAtControllerPosition();
            }
        }
    }

    private void SpawnObjectAtControllerPosition()
    {
        // TODO: TELL SERVER TO SPAWN OBJECT AT LOCATION
        ShootProjectileServerRPC(rightAnchor.position, rightAnchor.rotation.eulerAngles);
    }

    [ServerRpc]
    private void ShootProjectileServerRPC(Vector3 Position, Vector3 Rotation, ServerRpcParams serverRpcParams = default)
    {
        GameObject obj = Instantiate(testObjectPrefab, Position, Quaternion.Euler(Rotation));
        obj.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }
}
