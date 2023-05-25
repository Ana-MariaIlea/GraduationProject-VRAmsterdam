using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    public string SoundTag;

    private NetworkVariable<int> soundID = new NetworkVariable<int>(-1);
    private AudioSource source;

    public int SoundID
    {
        get
        {
            //Some other code
            return soundID.Value;
        }
        set
        {
            soundID.Value = value;
        }
    }

    [ClientRpc]
    public void PlaySoundClientRpc()
    {
        source.Play();
    }
}
