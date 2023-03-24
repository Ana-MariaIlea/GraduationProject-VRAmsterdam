using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireFriendlyCreature : AbstractFriendlyCreature
{
    private bool doesPlayerHaveFood = false;
    private LayerMask whatIsPlayer;
    [SerializeField] private Vector3 unbefriendedInitialSpace;
    [SerializeField] private Transform unbefriendedSpace;
    private GrabbableItem playerFood = null;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unbefriendedInitialSpace = transform.position;
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
                if (distance.magnitude < minDist)
                {
                    minDist = distance.magnitude;
                    playerTarget = hitCollidersSight[i].gameObject;
                    if (playerTarget.GetComponent<PlayerVRGrabbing>().GrabedItemID == ItemID.Food)
                    {
                        doesPlayerHaveFood = true;
                        playerFood = playerTarget.GetComponent<PlayerVRGrabbing>().GrabedItem;
                    }
                    else
                    {
                        doesPlayerHaveFood = false;
                        playerFood = null;
                    }
                }
            }


            if (doesPlayerHaveFood)
            {
                if (minDist < 2f)
                {
                    playerFood.gameObject.transform.SetParent(null);
                    playerTarget.GetComponent<PlayerVRGrabbing>().GrabedItemID = ItemID.None;
                    Destroy(playerFood.gameObject);
                    BefriendCreature();
                    //Send client RPC player does not have food 
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
            Vector3 distance = transform.position - unbefriendedInitialSpace;
            if (distance.magnitude < 2f)
            {
                meshAgent.SetDestination(transform.position);
            }
            else
            {
                meshAgent.SetDestination(unbefriendedInitialSpace);
            }
        }
    }
    protected override void HelpingBehaviour()
    {

    }

    public void InitializeCreatureData(List<FriendlyCreatureUnfriendedSpot> unfriendedSpots, LayerMask playerLayer)
    {
        whatIsPlayer = playerLayer;
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
