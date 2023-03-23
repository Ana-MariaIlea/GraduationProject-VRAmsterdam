using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireFriendlyCreature : AbstractFriendlyCreature
{
    private bool doesPlayerHaveFood = false;
    private LayerMask whatIsPlayer;
    private Transform unbefriendedInitialSpace;
    private Transform unbefriendedSpace;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unbefriendedInitialSpace = transform;
        type = CreatureType.Fire;
    }

    protected override void UnfriendedBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 20, whatIsPlayer);
        
        if (hitCollidersSight.Length >= 1)
        {
            float minDist = 20;
            for (int i = 0; i < hitCollidersSight.Length; i++)
            {
                Vector3 distance = transform.position - hitCollidersSight[i].transform.position;
                if(distance.magnitude < minDist)
                {
                    minDist = distance.magnitude;
                    playerTarget = hitCollidersSight[i].gameObject;
                }
            }

            if (doesPlayerHaveFood)
            {
                if (minDist < 2f)
                {
                    BefriendCreature();
                }
                else
                {
                    meshAgent.SetDestination(playerTarget.transform.position);
                }
            }
            else
            {
                if (minDist < 20f)
                {
                    meshAgent.SetDestination(unbefriendedSpace.position);
                }
                else
                {
                    meshAgent.SetDestination(transform.position);

                }
            }
        }
        else
        {
            Vector3 distance = transform.position - unbefriendedInitialSpace.transform.position;
            if (distance.magnitude < 2f)
            {
                meshAgent.SetDestination(transform.position);
            }
            else
            {
                meshAgent.SetDestination(unbefriendedInitialSpace.transform.position);
            }
        }
    }
    protected override void HelpingBehaviour()
    {

    }

    public void InitializeCreatureData(List<FriendlyCreatureUnfriendedSpot> unfriendedSpots)
    {
        float minDist = 20;
        for (int i = 0; i < unfriendedSpots.Count; i++)
        {
            Vector3 distance = transform.position - unfriendedSpots[i].transform.position;
            if (distance.magnitude < minDist)
            {
                minDist = distance.magnitude;
                unbefriendedSpace = unfriendedSpots[i].transform;
            }
        }
    }
}
