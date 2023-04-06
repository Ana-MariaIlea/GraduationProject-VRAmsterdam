using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void UnfriendedBehaviour()
    {

    }
    protected override void HelpingBehaviour()
    {

    }

    //------------------------------------------------------------------------------
    // </summary>
    //     This function is called instead of BefriendCreature to have some delay for animations
    // </summary>
    //------------------------------------------------------------------------------
    public void CreadureBefriendTransition()
    {
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
