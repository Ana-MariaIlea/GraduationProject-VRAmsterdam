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
    private List<FriendlyCreatureUnfriendedSpot> unfriendedSpots;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            unfriendedSpots = FindObjectsOfType<FriendlyCreatureUnfriendedSpot>().ToList();
            PlayerStateManager.Singleton.part1StartServer.AddListener(SpawnCreature);
            //SpawnCreature();
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
                fire.InitializeCreatureData(unfriendedSpots);
                break;
            case CreatureType.Water:
                WaterFriendlyCreature water = creature.GetComponent<WaterFriendlyCreature>();
                break;
            case CreatureType.Earth:
                EarthFriendlyCreature earth = creature.GetComponent<EarthFriendlyCreature>();
                break;
        }
    }
}
