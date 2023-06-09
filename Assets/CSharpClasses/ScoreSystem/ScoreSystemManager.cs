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
    [SerializeField] private GameObject KillsAndDeathsTitle;
    private NetworkList<PlayerIndividualScore> scoreList;
    private NetworkList<PlayerIndividualScore> finalScoreList;

    [System.Serializable]
    public struct ScoreBoard
    {
        public TMP_Text playerName;
        public TMP_Text playerScore;
        public TMP_Text playerKills;
        public TMP_Text playerDeaths;
    }
    public struct PlayerIndividualScore : INetworkSerializable, IEquatable<PlayerIndividualScore>
    {
        public ulong playerID;
        public float score;
        public int kills;
        public int deaths;

        public bool Equals(PlayerIndividualScore other)
        {
            return playerID == other.playerID && score == other.score && kills == other.kills && deaths == other.deaths;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerID);
            serializer.SerializeValue(ref score);
            serializer.SerializeValue(ref kills);
            serializer.SerializeValue(ref deaths);
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
                PlayerStateManager.Singleton.part1StartServer.AddListener(ResetScore);
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
        playerIndividualScore.kills = 0;
        playerIndividualScore.deaths = 0;
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
                newScore.kills = scoreList[i].kills;
                newScore.deaths = scoreList[i].deaths;
                scoreList[i] = newScore;
                return;
            }
        }
    }

    public void KillAddedToPlayer(ulong playerID, int scoreIncrease = 100)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == playerID)
            {
                PlayerIndividualScore newScore;
                newScore.playerID = scoreList[i].playerID;
                newScore.score = scoreList[i].score + scoreIncrease;
                newScore.kills = scoreList[i].kills + 1;
                newScore.deaths = scoreList[i].deaths;
                scoreList[i] = newScore;
                return;
            }
        }
    }

    public void DeathAddedToPlayer(ulong playerID)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == playerID)
            {
                PlayerIndividualScore newScore;
                newScore.playerID = scoreList[i].playerID;
                newScore.score = scoreList[i].score;
                newScore.kills = scoreList[i].kills;
                newScore.deaths = scoreList[i].deaths + 1;
                scoreList[i] = newScore;
                return;
            }
        }
    }

    private void CalcLeaderboard()
    {
        scoreBoardUI.SetActive(true);
        List<PlayerIndividualScore> sortedScores = new List<PlayerIndividualScore>();
        bool isGameCoOp = PlayerStateManager.Singleton.isPlayerCoOp;
        if (!isGameCoOp)
        {
            KillsAndDeathsTitle.SetActive(true);
        }
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
            if (!isGameCoOp)
            {
                scoreBoardElements[i].playerKills.text = finalScoreList[i].kills.ToString();
                scoreBoardElements[i].playerDeaths.text = finalScoreList[i].deaths.ToString();
            }
        }
        //ClientRpc Update UI with leaderboard
        CalcLeaderboardClientRpc(isGameCoOp);
    }

    [ClientRpc]
    private void CalcLeaderboardClientRpc(bool isGameCoOp)
    {
        StartCoroutine(ShowLeaderboardClient(isGameCoOp));
    }

    private IEnumerator ShowLeaderboardClient(bool isGameCoOp)
    {
        yield return new WaitForSeconds(.5f);

        scoreBoardUI.SetActive(true);

        if (!isGameCoOp)
        {
            KillsAndDeathsTitle.SetActive(true);
        }
        else
        {
            KillsAndDeathsTitle.SetActive(false);
        }

        for (int i = 0; i < finalScoreList.Count; i++)
        {
            scoreBoardElements[i].playerName.text = "Player " + finalScoreList[i].playerID;
            scoreBoardElements[i].playerScore.text = finalScoreList[i].score.ToString();
            if (!isGameCoOp)
            {
                scoreBoardElements[i].playerKills.text = finalScoreList[i].kills.ToString();
                scoreBoardElements[i].playerDeaths.text = finalScoreList[i].deaths.ToString();
            }
        }
    }

    void ResetScore()
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            PlayerIndividualScore newScore;
            newScore.playerID = scoreList[i].playerID;
            newScore.score = 0;
            newScore.kills = 0;
            newScore.deaths = 0;
            scoreList[i] = newScore;
        }
        for (int i = 0; i < scoreBoardElements.Count; i++)
        {
            scoreBoardElements[i].playerName.text = string.Empty;
            scoreBoardElements[i].playerScore.text = string.Empty;
            scoreBoardElements[i].playerKills.text = string.Empty;
            scoreBoardElements[i].playerDeaths.text = string.Empty;
        }
        scoreBoardUI.SetActive(false);
        ResetScoreClientRpc();
    }

    [ClientRpc]
    void ResetScoreClientRpc()
    {
        for (int i = 0; i < scoreBoardElements.Count; i++)
        {
            scoreBoardElements[i].playerName.text = string.Empty;
            scoreBoardElements[i].playerScore.text = string.Empty;
            scoreBoardElements[i].playerKills.text = string.Empty;
            scoreBoardElements[i].playerDeaths.text = string.Empty;
        }
        scoreBoardUI.SetActive(false);
    }
}
