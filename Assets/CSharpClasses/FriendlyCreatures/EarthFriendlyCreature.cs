using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthFriendlyCreature : AbstractFriendlyCreature
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        type = CreatureType.Earth;
    }
    protected override void HelpingBehaviour()
    {

    }
    public void CreadureBefriendTransition()
    {
        StartCoroutine(CreadureBefriendTransitionCorutine());
    }
    IEnumerator CreadureBefriendTransitionCorutine()
    {
        yield return null;

        BefriendCreature();
    }
}
