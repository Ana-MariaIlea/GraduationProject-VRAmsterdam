using Oculus.Platform.Samples.VrHoops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//------------------------------------------------------------------------------
// </summary>
//     Earth friendly creature class inherited from AbstractFriendlyCreature
// </summary>
//------------------------------------------------------------------------------
public class EarthFriendlyCreature : AbstractFriendlyCreature
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            type = CreatureType.Earth;
            base.OnNetworkSpawn();
        }
        else
        {
            this.enabled = false;
        }
    }
    protected override void HelpingBehaviour()
    {

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
}
