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
            
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part1StartServer.AddListener(SpawnGrabbableObject);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            base.OnNetworkDespawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part1StartServer.RemoveListener(SpawnGrabbableObject);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void SpawnGrabbableObject()
    {
        GameObject grabbableItem = Instantiate(grabbableObject, transform.position, Quaternion.identity);
        grabbableItem.GetComponent<NetworkObject>().Spawn(true);
        GrabbableItemManager.Singleton.AddGrabbableItem(grabbableItem.GetComponent<GrabbableItem>());
    }
}
