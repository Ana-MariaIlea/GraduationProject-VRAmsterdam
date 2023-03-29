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
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        type = CreatureType.Water;
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
