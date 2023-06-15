//Made by Ana-Maria Ilea

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Scripting;

//------------------------------------------------------------------------------
// </summary>
//     Fire friendly creature class inherited from AbstractFriendlyCreature
// </summary>
//------------------------------------------------------------------------------
public class FireFriendlyCreature : AbstractFriendlyCreature
{
    private bool doesPlayerHaveFood = false;
    [SerializeField] private Vector3 unbefriendedInitialSpace;

    //Spot the creature runs to when unfriended
    [SerializeField] private Transform unbefriendedSpace;

    PlayerVRGrabbing grabAux;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            unbefriendedInitialSpace = transform.position;
            type = CreatureType.Fire;
            base.OnNetworkSpawn();
        }
        else
        {
            this.enabled = false;
        }
    }

    //------------------------------------------------------------------------------
    // </summary>
    //     Custom behaviour when the creature is not befriended
    // </summary>
    //------------------------------------------------------------------------------
    protected override void UnfriendedBehaviour()
    {
        Collider[] hitCollidersSight = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Player"));

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

                    PlayerVRGrabbing[] grab = playerTarget.GetComponentsInChildren<PlayerVRGrabbing>();

                    //Check if the player holds food
                    if (grab.Length > 0)
                    {
                        if (grab[0].GrabedItemID == ItemID.Food)
                        {
                            doesPlayerHaveFood = true;
                            grabAux = grab[0];
                        }
                        else if (grab[1].GrabedItemID == ItemID.Food)
                        {
                            doesPlayerHaveFood = true;
                            grabAux = grab[1];
                        }
                        else
                        {
                            doesPlayerHaveFood = false;
                            grabAux = null;
                        }
                    }
                }
            }

            if (doesPlayerHaveFood)
            {
                // If the player has food, go to the player
                if (minDist > 2f)
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

    //------------------------------------------------------------------------------
    // </summary>
    //     This function is called instead of BefriendCreature to have some delay for animations
    // </summary>
    //------------------------------------------------------------------------------
    public void CreadureBefriendTransition(ulong playerID)
    {
        GameObject playerObj = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.gameObject;
        if (playerObj != null)
        {
            playerTarget = playerObj;
        }

        StartCoroutine(CreadureBefriendTransitionCorutine());
    }
    //------------------------------------------------------------------------------
    // </summary>
    //     Corutine used for animations when befriending the creature
    // </summary>
    //------------------------------------------------------------------------------
    IEnumerator CreadureBefriendTransitionCorutine()
    {
        yield return null;

        BefriendCreature();
    }
    //------------------------------------------------------------------------------
    // </summary>
    //     Funtion to get the closest unfriended spot
    // </summary>
    //------------------------------------------------------------------------------
    public void InitializeCreatureData(Transform unfriendedSpot, Transform helpingSpot)
    {
        unbefriendedSpace = unfriendedSpot;
        helpingSpace = helpingSpot;
    }
}
