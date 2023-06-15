//Made by Ana-Maria Ilea

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Script for inverse kinematics
// </summary>
//------------------------------------------------------------------------------
public class PlayerIKRigAnimator : NetworkBehaviour
{
    [SerializeField] private VRMap LeftHand;
    [SerializeField] private VRMap RightHand;
    [SerializeField] private VRMap Head;
    [SerializeField] private Transform HeadConstraint;
    [SerializeField] private Vector3 HeadBodyOffset;
    [SerializeField] private Transform BodyObject;

    public enum HandType
    {
        Left,
        Right,
        Head
    }

    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget;
        public Transform rigTarget;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        public void Map()
        {
            rigTarget.position = vrTarget.TransformPoint(positionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(rotationOffset);
        }

        public void SetPosition(Vector3 position, Vector3 rotation)
        {
            rigTarget.position = position;
            rigTarget.rotation = Quaternion.Euler(rotation);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            base.OnNetworkSpawn();
            HeadBodyOffset = BodyObject.position - HeadConstraint.position;
        }
    }

    private void LateUpdate()
    {
        if (IsOwner && IsClient)
        {
            MapBodyPosition();
            MapPosition(HandType.Left);
            MapPosition(HandType.Right);
            Head.Map();
        }
    }

    private void MapBodyPosition()
    {
        BodyObject.position = HeadConstraint.position + HeadBodyOffset;
        Vector3 newBodyPosition = BodyObject.position;
        MapBodyPositionServerRpc(newBodyPosition);
    }

    [ServerRpc]
    private void MapBodyPositionServerRpc(Vector3 NewPosition, ServerRpcParams serverRpcParams = default)
    {
        BodyObject.position = NewPosition;

        List<ulong> clientIDs = NetworkManager.Singleton.ConnectedClients.Keys.ToList();
        clientIDs.Remove(serverRpcParams.Receive.SenderClientId);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = clientIDs
            }
        };

        MapBodyPositionClientRpc(NewPosition, clientRpcParams);
    }

    [ClientRpc]
    private void MapBodyPositionClientRpc(Vector3 NewPosition, ClientRpcParams clientRpcParams = default)
    {
        BodyObject.position = NewPosition;
    }

    private void MapPosition(HandType type)
    {
        switch (type)
        {
            case HandType.Left:
                LeftHand.Map();
                MapPositionServerRpc(type, LeftHand.rigTarget.position, LeftHand.rigTarget.rotation.eulerAngles);
                break;
            case HandType.Right:
                RightHand.Map();
                MapPositionServerRpc(type, RightHand.rigTarget.position, RightHand.rigTarget.rotation.eulerAngles);
                break;
            case HandType.Head:
                Head.Map();
                MapPositionServerRpc(type, Head.rigTarget.position, Head.rigTarget.rotation.eulerAngles);
                break;
        }

    }

    [ServerRpc]
    private void MapPositionServerRpc(HandType type, Vector3 position, Vector3 rotation)
    {
        switch (type)
        {
            case HandType.Left:
                LeftHand.SetPosition(position, rotation);
                break;
            case HandType.Right:
                RightHand.SetPosition(position, rotation);
                break;
            case HandType.Head:
                Head.Map();
                MapPositionServerRpc(type, Head.rigTarget.position, Head.rigTarget.rotation.eulerAngles);
                break;
        }
        MapPositionClientRpc(type, position, rotation);
    }

    [ClientRpc]
    private void MapPositionClientRpc(HandType type, Vector3 position, Vector3 rotation)
    {
        switch (type)
        {
            case HandType.Left:
                LeftHand.SetPosition(position, rotation);
                break;
            case HandType.Right:
                RightHand.SetPosition(position, rotation);
                break;
            case HandType.Head:
                Head.Map();
                MapPositionServerRpc(type, Head.rigTarget.position, Head.rigTarget.rotation.eulerAngles);
                break;
        }
    }
}
