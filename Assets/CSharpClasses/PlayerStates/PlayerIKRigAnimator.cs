using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerIKRigAnimator : NetworkBehaviour
{
    [SerializeField] private VRMap LeftHand;
    [SerializeField] private VRMap RightHand;
    [SerializeField] private Transform HeadRigTarget;
    [SerializeField] private Transform HeadVRTarget;

    [SerializeField] private Vector3 HeadOffset = new Vector3(0, -0.8f, 0);

    public enum HandType
    {
        Left,
        Right
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

    private void Update()
    {
        if (IsOwner && IsClient)
        {
            MapPosition(HandType.Left);
            MapPosition(HandType.Right);
        }
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
        }
    }
}
