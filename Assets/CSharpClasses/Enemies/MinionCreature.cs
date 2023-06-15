//Made by Ana-Maria Ilea

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
    [SerializeField] private float minAttackSpeed = 1;
    [SerializeField] private float maxAttackSpeed = 5; 
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject minionModel;
    [SerializeField] private Vector3 minionModelScale;

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
        //Simple animation for spawning minion
        LeanTween.scale(minionModel, minionModelScale, 1f).setEaseOutBack();

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
        }
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
        GameObject projectile = Instantiate(projectilePrefab, ProjectileShootPoint.position, ProjectileShootPoint.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);

        //Wait for some time
        float attacktime = Random.Range(minAttackSpeed, maxAttackSpeed);
        yield return new WaitForSeconds(attacktime);
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
        //Invoke minion die event - the boss will recognize a minion has died
        minionDie?.Invoke();

        minionDie.RemoveAllListeners();

        if (attackCorutine != null)
        {
            //Stop attacking if the corutine is not finished
            StopCoroutine(attackCorutine);
        }
        StartCoroutine(MinionDieCorutine());
    }

    private IEnumerator MinionDieCorutine()
    {
        MinionDieClientRpc();
        yield return new WaitForSeconds(1);
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }

    [ClientRpc]
    private void MinionDieClientRpc()
    {
        LeanTween.scale(minionModel, Vector3.zero, 1f).setEaseOutBack();
    }
}
