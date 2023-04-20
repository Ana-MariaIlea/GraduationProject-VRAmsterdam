using Newtonsoft.Json.Linq;
using Oculus.Platform.Samples.VrHoops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;
using static Unity.Burst.Intrinsics.X86;

//------------------------------------------------------------------------------
// </summary>
//     Class used for deciding when Part 1 is complete. When all creatures are
//     collected Part 2 starts.
// </summary>
//------------------------------------------------------------------------------
public class PlayerCreatureHandler : NetworkBehaviour
{
    public static PlayerCreatureHandler Singleton;

    private NetworkList<PlayerCreatures> playerCreatures;

    public UnityEvent part2StartClient;
    public UnityEvent part2StartServer;

    public struct PlayerCreatures : INetworkSerializable, IEquatable<PlayerCreatures>
    {
        public ulong PlayerID;
        public int creaturesCollected;
        public bool isFireCretureCollected;
        public bool isWaterCretureCollected;
        public bool isEarthCretureCollected;

        public bool Equals(PlayerCreatures other)
        {
            return PlayerID == other.PlayerID && creaturesCollected == other.creaturesCollected &&
                isFireCretureCollected == other.isFireCretureCollected &&
                isWaterCretureCollected == other.isWaterCretureCollected &&
                isEarthCretureCollected == other.isEarthCretureCollected;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref creaturesCollected);
            serializer.SerializeValue(ref isFireCretureCollected);
            serializer.SerializeValue(ref isWaterCretureCollected);
            serializer.SerializeValue(ref isEarthCretureCollected);
        }
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            playerCreatures = new NetworkList<PlayerCreatures>(default);
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddEmptyPlayerStructure(ServerRpcParams serverRpcParams = default)
    {
        PlayerCreatures playerCreature = new PlayerCreatures();
        playerCreature.PlayerID = serverRpcParams.Receive.SenderClientId;
        playerCreature.creaturesCollected = 0;
        playerCreature.isFireCretureCollected = false;
        playerCreature.isWaterCretureCollected = false;
        playerCreature.isEarthCretureCollected = false;

        playerCreatures.Add(playerCreature);
    }

    public void RemovePlayerStructure(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < playerCreatures.Count; i++)
        {
            if (playerCreatures[i].PlayerID == serverRpcParams.Receive.SenderClientId)
            {
                playerCreatures.RemoveAt(i);
                return;
            }
        }

        throw new Exception("Player is not registered in the list");
    }

    public void CreatureCollected(CreatureType type, ServerRpcParams serverRpcParams = default)
    {
        PlayerCreatures aux = new PlayerCreatures();

        for (int i = 0; i < playerCreatures.Count; i++)
        {
            if (playerCreatures[i].PlayerID == serverRpcParams.Receive.SenderClientId)
            {
                aux.PlayerID = playerCreatures[i].PlayerID;
                aux.creaturesCollected = playerCreatures[i].creaturesCollected;
                aux.isFireCretureCollected = playerCreatures[i].isFireCretureCollected;
                aux.isWaterCretureCollected = playerCreatures[i].isWaterCretureCollected;
                aux.isEarthCretureCollected = playerCreatures[i].isEarthCretureCollected;
                switch (type)
                {
                    case CreatureType.Fire:

                        if (!playerCreatures[i].isFireCretureCollected)
                        {
                            aux.isFireCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                    case CreatureType.Water:
                        if (!playerCreatures[i].isWaterCretureCollected)
                        {
                            aux.isWaterCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                    case CreatureType.Earth:
                        if (!playerCreatures[i].isEarthCretureCollected)
                        {
                            aux.isEarthCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                }
                playerCreatures[i] = aux;
                Invoke("CheckPlayersCreatures", 1f);
                return;
            }
        }

        throw new Exception("Player is not registered in the list");
    }

    public bool CheckCollectedCreature(CreatureType type, ulong PlayerID)
    {
        bool isCreatureCollected = false;
        for (int i = 0; i < playerCreatures.Count; i++)
        {
            if (playerCreatures[i].PlayerID == PlayerID)
            {
                switch (type)
                {
                    case CreatureType.Fire:
                        if (playerCreatures[i].isFireCretureCollected)
                        {
                            isCreatureCollected = true;
                        }
                        break;
                    case CreatureType.Water:
                        if (playerCreatures[i].isWaterCretureCollected)
                        {
                            isCreatureCollected = true;
                        }
                        break;
                    case CreatureType.Earth:
                        if (playerCreatures[i].isEarthCretureCollected)
                        {
                            isCreatureCollected = true;
                        }
                        break;
                }
                break;
            }
        }

        return isCreatureCollected;
    }


    private void CheckPlayersCreatures()
    {
        for (int i = 0; i < playerCreatures.Count; i++)
        {
            Debug.Log(playerCreatures[i].creaturesCollected);
            if (playerCreatures[i].creaturesCollected < 3)
            {
                return;
            }
        }

        StartPart2ClientRpc();
        part2StartServer?.Invoke();
    }

    [ClientRpc]
    private void StartPart2ClientRpc()
    {
        part2StartClient?.Invoke();
    }
}
