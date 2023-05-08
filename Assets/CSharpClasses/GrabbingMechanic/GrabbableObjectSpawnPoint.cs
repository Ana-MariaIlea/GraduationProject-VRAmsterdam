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
            //SpawnGrabbableObject();
            PlayerStateManager.Singleton.part1StartServer.AddListener(SpawnGrabbableObject);
        }
    }

    private void SpawnGrabbableObject()
    {
        GameObject station = Instantiate(grabbableObject, transform.position, Quaternion.identity);
        station.GetComponent<NetworkObject>().Spawn(true);
    }
}
