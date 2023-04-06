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
            meshAgent = GetComponent<NavMeshAgent>();
            meshAgent.enabled = true;
            playerTarget = FindFirstObjectByType<PlayerCreatureHandler>().gameObject;
            InitializeCreatureVisuals();
            FindObjectOfType<PlayerStateManager>().part2Start.AddListener(Part2Start);
            base.OnNetworkSpawn();
        }
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
                GameObject visual = Instantiate(atlas.creatureVisualDatas[i].Mesh[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
                visual.GetComponent<NetworkObject>().Spawn();
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

    }

    public void BefriendCreature()
    {
        state = CreatureState.Befriended;
    }
}
