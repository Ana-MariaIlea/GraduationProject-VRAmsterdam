using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public enum CreatureType
{
    Fire,
    Water,
    Earth
}
public abstract class AbstractFriendlyCreature : MonoBehaviour
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


    // Start is called before the first frame update
    protected virtual void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        playerTarget = FindFirstObjectByType<PlayerCreatureHandler>().gameObject;
        InitializeCreatureVisuals();
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

    //public void InitializeCreatureData(CreatureType type)
    //{
    //    this.type = type;
    //}

    void InitializeCreatureVisuals()
    {
        CreatureAtlas atlas = GetComponent<CreatureAtlas>();
        for (int i = 0; i < atlas.creatureVisualDatas.Count; i++)
        {
            if (atlas.creatureVisualDatas[i].creatureType == type)
            {
                int randomIndex = Random.Range(0, atlas.creatureVisualDatas[i].Mesh.Count - 1);
                Instantiate(atlas.creatureVisualDatas[i].Mesh[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
                break;
            }
        }
    }

    protected virtual void UnfriendedBehaviour()
    {

    }
    protected virtual void BefriendedBehaviour()
    {
        Vector3 distance = transform.position - playerTarget.transform.position;
        if (distance.magnitude < 2f)
        {
            meshAgent.SetDestination(transform.position);
        }
        else
        {
            meshAgent.SetDestination(playerTarget.transform.position);
        }
    }
    protected virtual void HelpingBehaviour()
    {

    }

    public void BefriendCreature()
    {
        state = CreatureState.Befriended;
    }
}
