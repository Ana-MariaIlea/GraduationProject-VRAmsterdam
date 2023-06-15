//Made by Ana-Maria Ilea

using System.Collections.Generic;
using Unity.Netcode;

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
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].SoundID == ID)
            {
                if (specificPlayer)
                {
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
