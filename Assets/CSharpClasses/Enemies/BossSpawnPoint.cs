using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSpawnPoint : MonoBehaviour
{
    [SerializeField] bool isInitalSpawnPoint = false;
    [SerializeField] private GameObject bossPrefab;

    private List<MinionSpawnPoint> minionSpawnPoints;
    private List<BossSpawnPoint> bossSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        if (isInitalSpawnPoint)
        {
            //PlayerCreatureHandler.Singleton.part2StartServer.AddListener(SpawnBoss);
            minionSpawnPoints = FindObjectsOfType<MinionSpawnPoint>().ToList();
            bossSpawnPoints = FindObjectsOfType<BossSpawnPoint>().ToList();
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        GameObject creature;
        creature = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        //creature.GetComponent<NetworkObject>().Spawn(true);
        creature.GetComponent<BossCreature>().InitMinionSpawnpoints(minionSpawnPoints);
        creature.GetComponent<BossCreature>().InitBossSpawnpoints(bossSpawnPoints);
    }
}
