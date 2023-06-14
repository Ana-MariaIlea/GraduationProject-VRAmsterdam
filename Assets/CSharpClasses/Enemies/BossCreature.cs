using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BossCreature : NetworkBehaviour
{
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private List<BossStageElements> thresholds;
    [SerializeField] private GameObject bossModel;
    [SerializeField] private Transform ProjectileShootPoint;
    [SerializeField] private float attackRange = 3;
    [SerializeField] private float minAttackSpeed = 1;
    [SerializeField] private float maxAttackSpeed = 5;

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
        Shield,
        Die
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            meshAgent = GetComponent<NavMeshAgent>();
            health = MaxHealth;
        }
        else
        {
            this.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }
        LeanTween.scale(bossModel, Vector3.one, 1f).setEaseOutBack();
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
                //ShieldBehaviour();
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

            if (playerTarget == null)
            {
                meshAgent.SetDestination(transform.position);
                return;
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
        Vector3 destinationPos = new Vector3(playerTarget.position.x, playerTarget.position.y + 1f, playerTarget.position.z);
        ProjectileShootPoint.LookAt(destinationPos);
        GameObject projectile = Instantiate(thresholds[thresholdIndex].projectilePrefab, ProjectileShootPoint.position, ProjectileShootPoint.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);
        float attacktime = Random.Range(minAttackSpeed,maxAttackSpeed);
        yield return new WaitForSeconds(attacktime);
        attackCorutine = null;
    }

    private void ShieldBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Player"));

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
        SoundManager.Singleton.PlaySoundAllPlayers(GetComponent<SoundSource>().SoundID);
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
                MinionCreature minion = point.SpawnMinion(thresholds[thresholdIndex].minionTypeToSpawn);
                minion.minionDie.AddListener(MinionDied);
            }
            minionNumber = minionSpawnPoints.Count;
            thresholdIndex++;
            bossModel.SetActive(false);
            GetComponent<CapsuleCollider>().enabled = false;
            meshAgent.enabled = false;
            TurnOffBossModelClientRpc();
        }
    }

    [ClientRpc]
    private void TurnOffBossModelClientRpc()
    {
        //bossModel.SetActive(false);
        StartCoroutine(BossSpawningDespawing(false));
    }

    public void MinionDied()
    {
        minionNumber--;
        if (minionNumber == 0)
        {
            stage = BossStage.Fight;
            bossModel.SetActive(true);
            GetComponent<CapsuleCollider>().enabled = true;
            meshAgent.enabled = true;
            TurnOnBossModelClientRpc();
        }
    }

    [ClientRpc]
    private void TurnOnBossModelClientRpc()
    {
        //bossModel.SetActive(true);
        StartCoroutine(BossSpawningDespawing(true));
    }

    private IEnumerator BossSpawningDespawing(bool isSpawning)
    {
        if (isSpawning)
        {
            LeanTween.scale(bossModel, Vector3.one, 1f).setEaseOutBack();

        }
        else
        {
            LeanTween.scale(bossModel, Vector3.zero, 1f).setEaseInBack();
        }
        yield return null;
    }

    private void BossDie()
    {
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.GameEndServer();
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
        stage = BossStage.Die;
        meshAgent.SetDestination(transform.position);
        if (attackCorutine != null) StopCoroutine(attackCorutine);
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
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
