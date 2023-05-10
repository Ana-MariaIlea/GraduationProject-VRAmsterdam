using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GrabbableObjectSpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject grabbableObject;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            PlayerStateManager.Singleton.part1StartServer.AddListener(SpawnGrabbableObject);
        }
    }

    private void SpawnGrabbableObject()
    {
        GameObject grabbableItem = Instantiate(grabbableObject, transform.position, Quaternion.identity);
        grabbableItem.GetComponent<NetworkObject>().Spawn(true);
        GrabbableItemManager.Singleton.AddGrabbableItem(grabbableItem.GetComponent<GrabbableItem>());
    }
}
