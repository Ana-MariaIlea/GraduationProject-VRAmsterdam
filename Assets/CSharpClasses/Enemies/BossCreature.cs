using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossCreature : MonoBehaviour
{
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private List<BossStageElements> thresholds;
    [SerializeField] private GameObject shieldObject;
    float health;

    int thresholdIndex = 0;

    protected NavMeshAgent meshAgent;

    private BossStage stage = BossStage.Shield;

    private List<MinionSpawnPoint> minionSpawnPoints;
    private List<BossSpawnPoint> bossSpawnPoints;

    private int teleportSpawnPointIndex = -1;

    private Coroutine teleportCorutine = null;

    public struct BossStageElements
    {
        public string stageName;
        public float healthThreshhold;
        public GameObject projectilePrefab;
        public CreatureType minionTypeToSpawn;
    }

    public enum BossStage
    {
        Fight,
        Shield
    }

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case BossStage.Fight:
                FightBehaviour();
                break;
            case BossStage.Shield:
                ShieldBehaviour();
                break;
        }
    }

    private void FightBehaviour()
    {

    }

    private void ShieldBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Player"));

        // If there is a player in sight
        if (hitCollidersSight.Length >= 1)
        {
            TeleportBoss();
        }
    }

    private void TeleportBoss()
    {
        int rand = teleportSpawnPointIndex;
        while (rand == teleportSpawnPointIndex)
        {
            rand = Random.Range(0, bossSpawnPoints.Count);
        }
        teleportSpawnPointIndex = rand;

        if (teleportCorutine == null)
        {
            teleportCorutine = StartCoroutine(TeleportCorutine());
        }
    }

    private IEnumerator TeleportCorutine()
    {
        yield return null;
        transform.position = bossSpawnPoints[teleportSpawnPointIndex].transform.position;
        teleportCorutine = null;
    }

    private void DamangeBoss(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            BossDie();
            return;
        }
        if (health < thresholds[thresholdIndex].healthThreshhold)
        {
            stage = BossStage.Shield;
            foreach (var point in minionSpawnPoints)
            {
                point.SpawnMinion(thresholds[thresholdIndex].minionTypeToSpawn);
            }
            thresholdIndex++;
            shieldObject.SetActive(true);
            //Client RPC for visuals
        }

    }

    private void BossDie()
    {

    }

    public void InitMinionSpawnpoints(List<MinionSpawnPoint> minionSpawnPoints)
    {
        this.minionSpawnPoints = minionSpawnPoints;
    }

    public void InitBossSpawnpoints(List<BossSpawnPoint> bossSpawnPoints)
    {
        this.bossSpawnPoints = bossSpawnPoints;
    }
}
