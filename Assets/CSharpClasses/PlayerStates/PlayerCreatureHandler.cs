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

    private NetworkList<PlayerCreatures> playerCreatures = new NetworkList<PlayerCreatures>();

    public struct PlayerCreatures : INetworkSerializable, IEquatable<PlayerCreatures>
    {
        public ulong PlayerID;
        public int creaturesCollected;
        public bool isFireCretureCollected;
        public bool isWaterCretureCollected;
        public bool isEarthCretureCollected;

        public void ChangeValues(PlayerCreatures other)
        {
            if (other.PlayerID == PlayerID)
            {
                creaturesCollected = other.creaturesCollected;
                isFireCretureCollected = other.isFireCretureCollected;
                isWaterCretureCollected = other.isWaterCretureCollected;
                isEarthCretureCollected = other.isEarthCretureCollected;
            }
        }
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
        if (IsServer)
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(this);
            }
        }
    }

    [ServerRpc]
    public void AddEmptyPlayerStructureServerRpc(ulong playerID)
    {
        PlayerCreatures playerCreature = new PlayerCreatures();
        playerCreature.PlayerID = playerID;
        playerCreature.creaturesCollected = 0;
        playerCreature.isFireCretureCollected = false;
        playerCreature.isWaterCretureCollected = false;
        playerCreature.isEarthCretureCollected = false;

        playerCreatures.Add(playerCreature);
    }

    [ServerRpc]
    public void CreatureCollectedServerRpc(ulong PlayerID, CreatureType type)
    {
        PlayerCreatures aux = new PlayerCreatures();
        aux.PlayerID = 0;

        for (int i = 0; i < playerCreatures.Count; i++)
        {
            if (playerCreatures[i].PlayerID == PlayerID)
            {
                aux.PlayerID = playerCreatures[i].PlayerID;
                switch (type)
                {
                    case CreatureType.Fire:
                        if (!aux.isFireCretureCollected)
                        {
                            aux.isFireCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                    case CreatureType.Water:
                        if (!aux.isWaterCretureCollected)
                        {
                            aux.isWaterCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                    case CreatureType.Earth:
                        if (!aux.isEarthCretureCollected)
                        {
                            aux.isEarthCretureCollected = true;
                            aux.creaturesCollected++;
                        }
                        break;
                }
                playerCreatures[i].ChangeValues(aux);
                return;
            }
        }

        throw new Exception("Player is not registered in the list");
    }

    public bool CheckCollectedCreature(ulong PlayerID, CreatureType type)
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

    [ServerRpc]
    private void CheckPlayersCreaturesServerRpc()
    {
        for (int i = 0; i < playerCreatures.Count; i++)
        {
            if (playerCreatures[i].creaturesCollected < 3)
            {
                return;
            }
        }

        //Start part 2
    }
}
