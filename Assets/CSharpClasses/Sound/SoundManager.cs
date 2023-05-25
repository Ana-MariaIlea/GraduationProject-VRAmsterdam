using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
//using static UnityEditor.Progress;

public class SoundManager : NetworkBehaviour
{
    public static SoundManager Singleton { get; private set; }
    private List<SFX> sounds = new List<SFX>();
    private int indexID = -1;
    public struct SFX
    {
        public SoundSource sound;
        public int id;
    }

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
        SFX sfx=new SFX();
        sfx.sound = soundTag;
        soundTag.SoundID = indexID;
        sfx.id = indexID;
        indexID++;
        sounds.Add(sfx);
    }

    public void PlaySoundAllPlayers(int ID)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].id == ID)
            {
                sounds[i].sound.PlaySoundClientRpc();
                return;
            }
        }
    }

    public void RemoveSound(int soundID)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if(soundID == sounds[i].id)
            {
                sounds.RemoveAt(i);
                break;
            }
        }
    }
}
