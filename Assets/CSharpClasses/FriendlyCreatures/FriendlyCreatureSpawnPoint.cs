using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FriendlyCreatureSpawnPoint : MonoBehaviour
{
    [SerializeField] private CreatureType creatureType;
    [SerializeField] private GameObject frindlyCreaturePrefab;
    [SerializeField] private LayerMask whatIsPlayer;
    private List<FriendlyCreatureUnfriendedSpot> unfriendedSpots;
    // Start is called before the first frame update
    void Start()
    {
        unfriendedSpots = FindObjectsOfType<FriendlyCreatureUnfriendedSpot>().ToList();
        SpawnCreature();
    }

    private void SpawnCreature()
    {
        GameObject creature;
        creature = Instantiate(frindlyCreaturePrefab, transform.position, Quaternion.identity);
        switch (creatureType)
        {
            case CreatureType.Fire:
                FireFriendlyCreature fire = creature.GetComponent<FireFriendlyCreature>();
                fire.InitializeCreatureData(unfriendedSpots, whatIsPlayer);
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
