using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundManager : NetworkBehaviour
{
    public static SoundManager Singleton { get; private set; }
    private List<SoundSource> sounds = new List<SoundSource>();
    private int indexID = -1;

    private void Awake()
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
    public void AddSound(SoundSource soundTag)
    {
        if (IsServer)
        {
            soundTag.SoundID = indexID;
            indexID++;
        }
        sounds.Add(soundTag);
    }

    public void PlaySoundAllPlayers(int ID, bool specificPlayer = false, ulong PlayerID = 0)
    {
        Debug.Log("Send all players sound server rpc " + ID);
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].SoundID == ID)
            {
                if (specificPlayer)
                {
                    Debug.Log("Send specific sound");
                    sounds[i].PlaySoundClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { PlayerID } } });
                }
                else
                {
                    sounds[i].PlaySoundClientRpc();
                }
                return;
            }
        }
    }

    public void RemoveSound(int soundID)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (soundID == sounds[i].SoundID)
            {
                sounds.RemoveAt(i);
                break;
            }
        }
    }
}
