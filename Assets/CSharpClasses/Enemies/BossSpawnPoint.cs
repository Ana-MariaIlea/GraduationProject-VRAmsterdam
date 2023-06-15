//Made by Ana-Maria Ilea

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
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.AddListener(SpawnBoss);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
            
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (isInitalSpawnPoint && IsServer)
        {
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.RemoveListener(SpawnBoss);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }

        }
    }

    private void SpawnBoss()
    {
        //Get minion and boss spawn points
        minionSpawnPoints = FindObjectsOfType<MinionSpawnPoint>().ToList();
        bossSpawnPoints = FindObjectsOfType<BossSpawnPoint>().ToList();

        //Spawn and initialize boss
        GameObject creature = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        creature.GetComponent<NetworkObject>().Spawn(true);
        creature.GetComponent<BossCreature>().InitMinionSpawnpoints(minionSpawnPoints);
        creature.GetComponent<BossCreature>().InitBossSpawnpoints(bossSpawnPoints);
    }
}
