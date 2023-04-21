using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinionSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;



    public void SpawnMinion(CreatureType creatureType)
    {
        GameObject creature;
        creature = Instantiate(minionPrefab, transform.position, Quaternion.identity);
        creature.GetComponent<MinionCreature>().CCreatureType = creatureType;
        //creature.GetComponent<NetworkObject>().Spawn(true);
    }
}
