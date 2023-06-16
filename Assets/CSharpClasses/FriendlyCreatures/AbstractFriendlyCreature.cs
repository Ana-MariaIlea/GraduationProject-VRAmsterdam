//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

//------------------------------------------------------------------------------
// </summary>
//     Enum used for defining creature types
// </summary>
//------------------------------------------------------------------------------
public enum CreatureType
{
    None,
    Fire,
    Water,
    Earth
}
//------------------------------------------------------------------------------
// </summary>
//     This class is the base class for all friendly creatures.
// </summary>
//------------------------------------------------------------------------------
public abstract class AbstractFriendlyCreature : NetworkBehaviour
{
    public enum CreatureState
    {
        Unfriended,
        Befriended,
        Helping
    }
    protected NavMeshAgent meshAgent;

    protected CreatureState state = CreatureState.Unfriended;
    protected CreatureType type;
    protected GameObject playerTarget;
    protected Transform helpingSpace;

    private GameObject visuals;

    public CreatureType CCreatureType
    {
        get
        {
            return type;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //The script runs on the server
            base.OnNetworkSpawn();

            meshAgent = GetComponent<NavMeshAgent>();
            InitializeCreatureVisuals();

            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.part2PlayerCoOpStartServer.AddListener(Part2Start);
                PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.AddListener(Part2Start);
                PlayerStateManager.Singleton.endingStartServer.AddListener(GameEnd);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
        else
        {
            //If it is not server, disable the script
            this.enabled = false;
        }
    }

    private void GameEnd()
    {
        //Unsubscribed from events
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.part2PlayerCoOpStartServer.RemoveListener(Part2Start);
            PlayerStateManager.Singleton.part2PlayerVsPlayerStartServer.RemoveListener(Part2Start);
            PlayerStateManager.Singleton.endingStartServer.RemoveListener(GameEnd);
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }

        //Despawn and destroy the visuals
        visuals.GetComponent<NetworkObject>().Despawn();
        Destroy(visuals);

        //Destroy and despawn the creature
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case CreatureState.Unfriended:
                UnfriendedBehaviour();
                break;
            case CreatureState.Befriended:
                //If the player is null - player disconnects - creature starts helping
                if (playerTarget == null)
                {
                    state = CreatureState.Helping;
                    return;
                }
                BefriendedBehaviour();
                break;
            case CreatureState.Helping:
                HelpingBehaviour();
                break;
        }
    }

    void Part2Start()
    {
        state = CreatureState.Helping;
    }

    void InitializeCreatureVisuals()
    {
        // Get the atlas
        CreatureAtlas atlas = GetComponent<CreatureAtlas>();

        for (int i = 0; i < atlas.creatureVisualDatas.Count; i++)
        {
            if (atlas.creatureVisualDatas[i].creatureType == type)
            {
                // If the atlas contains a creature with the type, choose a random prefab from list 
                int randomIndex = Random.Range(0, atlas.creatureVisualDatas[i].Mesh.Count - 1);

                // Instantiate it with a random prfab from list
                visuals = Instantiate(atlas.creatureVisualDatas[i].Mesh[randomIndex], transform.position, transform.rotation);

                visuals.GetComponent<NetworkObject>().Spawn(true);
                visuals.GetComponent<NetworkObject>().TrySetParent(transform);
                break;
            }
        }
    }

    //------------------------------------------------------------------------------
    // </summary>
    //      This function will be called when the creature is not befriended in Part 1
    // </summary>
    //------------------------------------------------------------------------------
    protected virtual void UnfriendedBehaviour()
    {
        // Override the method in other classes
    }


    //------------------------------------------------------------------------------
    // </summary>
    //      This function will be called after the creature has been befriended in Part 1
    //          This function can be overrided, though all creature behave the same after befriending
    // </summary>
    //------------------------------------------------------------------------------
    protected virtual void BefriendedBehaviour()
    {
        // Get the distance between the creature and the player
        Vector3 distance = transform.position - playerTarget.transform.position;

        // If the creature is close to the player, the agent will not move,
        //      else the creature will move to the player
        if (distance.magnitude < 2f)
        {
            meshAgent.SetDestination(transform.position);
        }
        else
        {
            meshAgent.SetDestination(playerTarget.transform.position);
        }
    }

    //------------------------------------------------------------------------------
    // </summary>
    //      This function will be called in Part 2
    // </summary>
    //------------------------------------------------------------------------------
    protected virtual void HelpingBehaviour()
    {
        //Get distance to the helping space
        Vector3 distance = transform.position - helpingSpace.position;
        float minDist = distance.magnitude;

        if (minDist < 20f)
        {
            //Go to the helping space
            meshAgent.SetDestination(helpingSpace.position);
        }
        else
        {
            //if it is at the helping space, set destination of the agent to self
            meshAgent.SetDestination(transform.position);
        }
    }

    public void BefriendCreature()
    {
        state = CreatureState.Befriended;
    }
}
