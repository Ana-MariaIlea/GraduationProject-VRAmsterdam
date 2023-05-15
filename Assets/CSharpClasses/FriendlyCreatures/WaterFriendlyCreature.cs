using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//------------------------------------------------------------------------------
// </summary>
//     Water friendly creature class inherited from AbstractFriendlyCreature
// </summary>
//------------------------------------------------------------------------------
public class WaterFriendlyCreature : AbstractFriendlyCreature
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            type = CreatureType.Water;
            base.OnNetworkSpawn();
        }
        else
        {
            this.enabled = false;
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

    public void InitializeCreatureData(Transform helpingSpot)
    {
        helpingSpace = helpingSpot;
    }

}
