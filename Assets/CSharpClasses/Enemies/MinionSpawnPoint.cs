using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinionSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;

    public MinionCreature SpawnMinion(CreatureType creatureType)
    {
        GameObject creature;
        creature = Instantiate(minionPrefab, transform.position, Quaternion.identity);
        MinionCreature minion = creature.GetComponent<MinionCreature>();
        minion.CCreatureType = creatureType;
        //creature.GetComponent<NetworkObject>().Spawn(true);
        return minion;
    }
}
