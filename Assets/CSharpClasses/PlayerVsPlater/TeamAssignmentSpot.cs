using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamAssignmentSpot : NetworkBehaviour
{
    public PossibleTeams team;
    [SerializeField] private GameObject visuals;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
            GetComponent<BoxCollider>().enabled = true;
        visuals.SetActive(true);
    }
}

public enum PossibleTeams
{
    Team1,
    Team2
}
