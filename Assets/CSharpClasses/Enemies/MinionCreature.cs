using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MinionCreature : NetworkBehaviour
{
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private Transform ProjectileShootPoint;
    [SerializeField] private float attackRange = 10;
    [SerializeField] private float attackSpeed = 4;
    [SerializeField] private GameObject projectilePrefab;

    [HideInInspector] public UnityEvent minionDie;

    float health;

    private CreatureType creatureType;

    protected NavMeshAgent meshAgent;

    private Transform playerTarget;

    private Coroutine attackCorutine = null;

    public CreatureType CCreatureType
    {
        get { return creatureType; }
        set { creatureType = value; }
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
        }
    }

    private void InitiallizeMinion()
    {

    } 

    // Update is called once per frame
    void Update()
    {
        MinionAttack();
    }

    private void MinionAttack()
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
        Vector3 destinationPos = new Vector3(playerTarget.position.x, playerTarget.position.y + 1f, playerTarget.position.z);
        ProjectileShootPoint.LookAt(destinationPos);
        GameObject projectile = Instantiate(projectilePrefab, ProjectileShootPoint.position, ProjectileShootPoint.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);
        yield return new WaitForSeconds(attackSpeed);
        attackCorutine = null;
    }
    public void DamangeMinion(float damage)
    {
        health -= damage;
        SoundManager.Singleton.PlaySoundAllPlayers(GetComponent<SoundSource>().SoundID);
        if (health < 0)
        {
            MinionDie();
            return;
        }
    }

    private void MinionDie()
    {
        minionDie?.Invoke();

        minionDie.RemoveAllListeners();

        if (attackCorutine != null)
        {
            StopCoroutine(attackCorutine);
        }
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}
