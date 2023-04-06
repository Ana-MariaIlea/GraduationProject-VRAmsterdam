using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Fire friendly creature class inherited from AbstractFriendlyCreature
// </summary>
//------------------------------------------------------------------------------
public class FireFriendlyCreature : AbstractFriendlyCreature
{
    private bool doesPlayerHaveFood = false;
    private LayerMask whatIsPlayer;
    [SerializeField] private Vector3 unbefriendedInitialSpace;

    //Spot the creature runs to when unfriended
    [SerializeField] private Transform unbefriendedSpace;

    private GrabbableItem playerFood = null;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            unbefriendedInitialSpace = transform.position;
            type = CreatureType.Fire;
        }
    }

    //------------------------------------------------------------------------------
    // </summary>
    //     Custom behaviour when the creature is not befriended
    // </summary>
    //------------------------------------------------------------------------------
    protected override void UnfriendedBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 20, whatIsPlayer);

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
                // If the player has food, go to the player
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
                // If the player doesn't have food, run to the unfriended spot
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
            //If there are no players go to the spawn point
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
    //------------------------------------------------------------------------------
    // </summary>
    //     Funtion to get the closest unfriended spot
    // </summary>
    //------------------------------------------------------------------------------
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
