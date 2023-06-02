using Oculus.Platform.Samples.VrHoops;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

//------------------------------------------------------------------------------
// </summary>
//     Score system class. This class handles scoreing for all the players
//     individually and in teams. Score is increased when a projectile 
//     collisdes with an enemy.
// </summary>
//------------------------------------------------------------------------------
public class ScoreSystemManager : NetworkBehaviour
{
    public static ScoreSystemManager Singleton { get; private set; }
    [SerializeField] private List<ScoreBoard> scoreBoardElements;
    [SerializeField] private GameObject scoreBoardUI;
    private NetworkList<PlayerIndividualScore> scoreList;
    private NetworkList<PlayerIndividualScore> finalScoreList;

    [System.Serializable]
    public struct ScoreBoard
    {
        public TMP_Text playerName;
        public TMP_Text playerScore;
    }
    public struct PlayerIndividualScore : INetworkSerializable, IEquatable<PlayerIndividualScore>
    {
        public ulong playerID; 
        public float score;

        public bool Equals(PlayerIndividualScore other)
        {
            return playerID == other.playerID && score == other.score;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerID);
            serializer.SerializeValue(ref score);
        }
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            scoreList = new NetworkList<PlayerIndividualScore>();
            finalScoreList = new NetworkList<PlayerIndividualScore>();
        }
        else
        {
            Destroy(this);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            base.OnNetworkSpawn();
            if (PlayerStateManager.Singleton)
            {
                PlayerStateManager.Singleton.endingStartServer.AddListener(CalcLeaderboard);
            }
            else
            {
                Debug.LogError("No PlayerStateManager in the scene");
            }
        }
    }

    public void NewPlayerConnected(ServerRpcParams serverRpcParams = default)
    {
        PlayerIndividualScore playerIndividualScore = new PlayerIndividualScore();
        playerIndividualScore.playerID = serverRpcParams.Receive.SenderClientId;
        playerIndividualScore.score = 0;
        scoreList.Add(playerIndividualScore);
    }

    public void PlayerDisonnected(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == serverRpcParams.Receive.SenderClientId)
            {
                scoreList.RemoveAt(i);
                return;
            }
        }
    }

    public void ScoreAddedToPlayer(ulong playerID, int scoreIncrease = 100)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == playerID)
            {
                PlayerIndividualScore newScore;
                newScore.playerID = scoreList[i].playerID;
                newScore.score = scoreList[i].score + scoreIncrease;
                scoreList[i] = newScore;
                return;
            }
        }
    }

    private void CalcLeaderboard()
    {
        scoreBoardUI.SetActive(true);
        List<PlayerIndividualScore> sortedScores = new List<PlayerIndividualScore>();
        for (int i = 0; i < scoreList.Count; i++)
        {
            sortedScores.Add(scoreList[i]);
        }

        sortedScores.Sort((p1, p2) => p1.score.CompareTo(p2.score));

        for (int i = 0; i < sortedScores.Count; i++)
        {
            finalScoreList.Add(scoreList[i]);
        }
        for (int i = 0; i < finalScoreList.Count; i++)
        {
            scoreBoardElements[i].playerName.text = "Player " + finalScoreList[i].playerID;
            scoreBoardElements[i].playerScore.text = finalScoreList[i].score.ToString();
        }
        //ClientRpc Update UI with leaderboard
        CalcLeaderboardClientRpc();
    }

    [ClientRpc]
    private void CalcLeaderboardClientRpc()
    {
        StartCoroutine(ShowLeaderboardClient());
    }

    private IEnumerator ShowLeaderboardClient()
    {
        yield return new WaitForSeconds(.5f);

        scoreBoardUI.SetActive(true);

        for (int i = 0; i < finalScoreList.Count; i++)
        {
            scoreBoardElements[i].playerName.text = "Player " + finalScoreList[i].playerID;
            scoreBoardElements[i].playerScore.text = finalScoreList[i].score.ToString();
        }
    }
}
