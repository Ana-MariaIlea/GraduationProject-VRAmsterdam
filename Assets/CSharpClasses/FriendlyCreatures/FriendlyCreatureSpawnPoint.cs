using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

//------------------------------------------------------------------------------
// </summary>
//     Spawn point class for the friendly creatures
// </summary>
//------------------------------------------------------------------------------
public class FriendlyCreatureSpawnPoint : NetworkBehaviour
{
    [SerializeField] private CreatureType creatureType;
    [SerializeField] private GameObject frindlyCreaturePrefab;
    [SerializeField] private Transform helpingSpot;
    [SerializeField] private Transform unfriendedSpot;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part1StartServer.AddListener(SpawnCreature);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void SpawnCreature()
    {
        GameObject creature;
        creature = Instantiate(frindlyCreaturePrefab, transform.position, Quaternion.identity);
        creature.GetComponent<NetworkObject>().Spawn(true);
        switch (creatureType)
        {
            case CreatureType.Fire:
                FireFriendlyCreature fire = creature.GetComponent<FireFriendlyCreature>();
                fire.InitializeCreatureData(unfriendedSpot, helpingSpot);
                break;
            case CreatureType.Water:
                WaterFriendlyCreature water = creature.GetComponent<WaterFriendlyCreature>();
                water.InitializeCreatureData(helpingSpot);
                break;
            case CreatureType.Earth:
                EarthFriendlyCreature earth = creature.GetComponent<EarthFriendlyCreature>();
                earth.InitializeCreatureData(helpingSpot);
                break;
        }
    }
}
