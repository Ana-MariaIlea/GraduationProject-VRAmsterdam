using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class BossSpawnPoint : NetworkBehaviour
{
    [SerializeField] bool isInitalSpawnPoint = false;
    [SerializeField] private GameObject bossPrefab;

    private List<MinionSpawnPoint> minionSpawnPoints;
    private List<BossSpawnPoint> bossSpawnPoints;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (isInitalSpawnPoint && IsServer)
        {
            PlayerCreatureHandler.Singleton.part2StartServer.AddListener(SpawnBoss);
        }
    }

    private void SpawnBoss()
    {
        minionSpawnPoints = FindObjectsOfType<MinionSpawnPoint>().ToList();
        bossSpawnPoints = FindObjectsOfType<BossSpawnPoint>().ToList();
        GameObject creature;
        creature = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        //creature.GetComponent<NetworkObject>().Spawn(true);
        creature.GetComponent<BossCreature>().InitMinionSpawnpoints(minionSpawnPoints);
        creature.GetComponent<BossCreature>().InitBossSpawnpoints(bossSpawnPoints);
    }
}
