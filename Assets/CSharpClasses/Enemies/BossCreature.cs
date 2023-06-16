//Made by Ana-Maria Ilea

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
            //The script runs on the server
            base.OnNetworkSpawn();
            meshAgent = GetComponent<NavMeshAgent>();
            health = MaxHealth;
        }
        else
        {
            //If it is not server, disable the script
            this.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        //Add simple animation for boss spawning
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
            
            if (minDist > attackRange)
            {
                //If the player is in not the range, move to closest player
                meshAgent.SetDestination(playerTarget.position);
            }
            else
            {
                //if the player is in the range, attack
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
        //Get direction and orientation of the projectile
        //Add offset to the player y position to the direction hits around the centre of the chest
        Vector3 destinationPos = new Vector3(playerTarget.position.x, playerTarget.position.y + 1f, playerTarget.position.z);
        ProjectileShootPoint.LookAt(destinationPos);

        //Spawn projectile
        GameObject projectile = Instantiate(thresholds[thresholdIndex].projectilePrefab, ProjectileShootPoint.position, ProjectileShootPoint.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);

        //Wait for some time
        float attacktime = Random.Range(minAttackSpeed,maxAttackSpeed);
        yield return new WaitForSeconds(attacktime);
        attackCorutine = null;
    }

    private void ShieldBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Player"));

        // If there is a player in sight teleport
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
            //If the boss health reches a threshold change behavior
            stage = BossStage.Shield;

            //Spawn minions
            foreach (var point in minionSpawnPoints)
            {
                MinionCreature minion = point.SpawnMinion(thresholds[thresholdIndex].minionTypeToSpawn);
                minion.minionDie.AddListener(MinionDied);
            }
            minionNumber = minionSpawnPoints.Count;
            
            //Increase the threshold index
            thresholdIndex++;

            //Make the boss dissaprer - replaced the teleporting feature
            bossModel.SetActive(false);
            GetComponent<CapsuleCollider>().enabled = false;
            meshAgent.enabled = false;
            TurnOffBossModelClientRpc();
        }
    }

    [ClientRpc]
    private void TurnOffBossModelClientRpc()
    {
        StartCoroutine(BossSpawningDespawing(false));
    }

    [ClientRpc]
    private void TurnOnBossModelClientRpc()
    {
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


    public void MinionDied()
    {
        minionNumber--;
        if (minionNumber == 0)
        {
            //If all the minions have died, the boss starts to attack
            stage = BossStage.Fight;
            bossModel.SetActive(true);
            GetComponent<CapsuleCollider>().enabled = true;
            meshAgent.enabled = true;
            TurnOnBossModelClientRpc();
        }
    }

    private void BossDie()
    {
        if (PlayerStateManager.Singleton)
        {
            //Trigger event for game end
            PlayerStateManager.Singleton.GameEndServer();
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }

        //Set oss for ending
        stage = BossStage.Die;
        meshAgent.SetDestination(transform.position);
        if (attackCorutine != null) StopCoroutine(attackCorutine);

        //Despawn and destory boss
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
