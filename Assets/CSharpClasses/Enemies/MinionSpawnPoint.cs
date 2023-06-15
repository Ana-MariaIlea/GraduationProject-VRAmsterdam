//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;

public class MinionSpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject minionPrefab;

    public MinionCreature SpawnMinion(CreatureType creatureType)
    {
        GameObject creature = Instantiate(minionPrefab, transform.position, Quaternion.identity);
        MinionCreature minion = creature.GetComponent<MinionCreature>();
        minion.CCreatureType = creatureType;
        creature.GetComponent<NetworkObject>().Spawn(true);
        return minion;
    }
}
