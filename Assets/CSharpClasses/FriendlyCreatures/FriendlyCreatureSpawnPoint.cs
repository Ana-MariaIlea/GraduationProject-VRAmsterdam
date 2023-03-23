using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCreatureSpawnPoint : MonoBehaviour
{
    [SerializeField] private CreatureType creatureType;
    [SerializeField] private GameObject FrindlyCreaturePrefab;
    // Start is called before the first frame update
    void Start()
    {
        SpawnCreature();
    }

    private void SpawnCreature()
    {
        GameObject creature;
        creature = Instantiate(FrindlyCreaturePrefab, transform.position, Quaternion.identity);
        switch (creatureType)
        {
            case CreatureType.Fire:
                FireFriendlyCreature fire = creature.AddComponent<FireFriendlyCreature>();
                fire.InitializeCreatureData(creatureType);
                break;
            case CreatureType.Water:
                WaterFriendlyCreature water = creature.AddComponent<WaterFriendlyCreature>();
                water.InitializeCreatureData(creatureType);
                break;
            case CreatureType.Earth:
                EarthFriendlyCreature earth = creature.AddComponent<EarthFriendlyCreature>();
                earth.InitializeCreatureData(creatureType);
                break;
        }
    }
}
