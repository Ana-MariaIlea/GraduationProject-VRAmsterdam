using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static ScoreSystemManager;

public class PlayerVsPlayerTimer : NetworkBehaviour
{
    public static PlayerVsPlayerTimer Singleton { get; private set; }
    [SerializeField] private float gameTime = 180f;

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
                StartCoroutine(StartGameTimerCorutine());
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    private IEnumerator StartGameTimerCorutine()
    {
        yield return new WaitForSeconds(gameTime);
        if (PlayerStateManager.Singleton)
        {
            PlayerStateManager.Singleton.GameEndServer();
        }
        else
        {
            Debug.LogError("No PlayerStateManager in the scene");
        }
    }

}
