using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerVsPlayerTimer : NetworkBehaviour
{
    public static PlayerVsPlayerTimer Singleton { get; private set; }
    [SerializeField] private float gameTime = 180f;
    [SerializeField] private GameObject TimerObject;
    [SerializeField] private TMP_Text TimerText;

    float timer;

    private List<PlayerReady> playerReadies = new List<PlayerReady>();
    public struct PlayerReady
    {
        public ulong playerID;
        public bool isReady;
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
    public void NewPlayerConnected(ServerRpcParams serverRpcParams = default)
    {
        if (IsServer)
        {
            for (int i = 0; i < playerReadies.Count; i++)
            {
                if (playerReadies[i].playerID == serverRpcParams.Receive.SenderClientId)
                {
                    return;
                }
            }
            PlayerReady playerIndividualScore = new PlayerReady();
            playerIndividualScore.playerID = serverRpcParams.Receive.SenderClientId;
            playerIndividualScore.isReady = true;
            playerReadies.Add(playerIndividualScore);

            if (playerReadies.Count == NetworkManager.Singleton.ConnectedClients.Count)
            {
                //Start player vs player part 2
                if (PlayerStateManager.Singleton)
                {
                    PlayerStateManager.Singleton.StartPart2PlayerVsPlayerServer();
                    timer = gameTime;
                    TimerObject.SetActive(true);
                    DisplayTimerClientRpc();
                    StartCoroutine(StartGameTimerCorutine());
                }
                else
                {
                    Debug.LogError("No PlayerStateManager in the scene");
                }
            }
        }
    }

    [ClientRpc]
    private void DisplayTimerClientRpc()
    {
        TimerObject.SetActive(true);
    }

    private IEnumerator StartGameTimerCorutine()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            int min = (int)(timer / 60);
            int sec = (int)(timer - min * 60);
            if (sec < 10)
            {
                TimerText.text = min.ToString() + ":0" + sec.ToString();
            }
            else
            {
                TimerText.text = min.ToString() + ":" + sec.ToString();
            }
            UpdateTimerClientRpc(min, sec);
            yield return null;
        }
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.GameEndServer();
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }

    [ClientRpc]
    private void UpdateTimerClientRpc(int min, int sec)
    {
        if (sec < 10)
        {
            TimerText.text = min.ToString() + ":0" + sec.ToString();
        }
        else
        {
            TimerText.text = min.ToString() + ":" + sec.ToString();
        }
    }

}
