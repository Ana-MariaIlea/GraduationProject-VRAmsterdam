using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargingStation : NetworkBehaviour
{
    [SerializeField] private CreatureType creatureType;

    public CreatureType CCreatureType
    {
        get
        {
            //Some other code
            return creatureType;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.AddListener(GameEnd);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            base.OnNetworkDespawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.RemoveListener(GameEnd);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private void GameEnd()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
