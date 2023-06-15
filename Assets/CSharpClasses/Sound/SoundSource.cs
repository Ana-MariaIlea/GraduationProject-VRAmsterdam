//Made by Ana-Maria Ilea

using Unity.Netcode;
using UnityEngine;

public class SoundSource : NetworkBehaviour
{
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SoundManager.Singleton.AddSound(this);
        if (IsClient)
        {
            source = GetComponent<AudioSource>();
        }
    }

    [ClientRpc]
    public void PlaySoundClientRpc(ClientRpcParams clientRpcParams = default)
    {
        source.Play();
    }
}
