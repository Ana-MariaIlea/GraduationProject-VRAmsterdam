using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossCreature : MonoBehaviour
{
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private List<BossStageElements> thresholds;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private Transform ProjectileShootPoint;
    [SerializeField] private float attackRange = 10;

    private float health;

    private int thresholdIndex = 0;
    private int minionNumber = 0;

    protected NavMeshAgent meshAgent;

    private BossStage stage = BossStage.Fight;

    private List<MinionSpawnPoint> minionSpawnPoints;
    private List<BossSpawnPoint> bossSpawnPoints;

    private int teleportSpawnPointIndex = -1;

    private Transform playerTarget;

    private Coroutine attackCorutine = null;
    private Coroutine teleportCorutine = null;

    [System.Serializable]
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
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 200, LayerMask.GetMask("Player"));

        // If there is a player in sight
        if (hitCollidersSight.Length >= 1)
        {
            float minDist = 20;

            //Get the closest player
            for (int i = 0; i < hitCollidersSight.Length; i++)
            {
                Vector3 distance = transform.position - hitCollidersSight[i].transform.position;
                if (distance.magnitude < minDist)
                {
                    minDist = distance.magnitude;
                    playerTarget = hitCollidersSight[i].transform;
                }
            }

            // If the player has food, go to the player
            if (minDist > attackRange)
            {
                meshAgent.SetDestination(playerTarget.position);
            }
            else
            {
                meshAgent.SetDestination(transform.position);
                if (attackCorutine == null)
                {
                    attackCorutine = StartCoroutine(AttackCorutine());
                }
            }
        }
    }

    private IEnumerator AttackCorutine()
    {
        ProjectileShootPoint.LookAt(playerTarget);
        Instantiate(thresholds[thresholdIndex].projectilePrefab, ProjectileShootPoint.position, ProjectileShootPoint.rotation);
        yield return new WaitForSeconds(2f);
        attackCorutine = null;
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

    public void DamangeBoss(float damage)
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

    public void MinionDied()
    {
        minionNumber--;
        if (minionNumber == 0)
        {
            stage = BossStage.Fight;
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
