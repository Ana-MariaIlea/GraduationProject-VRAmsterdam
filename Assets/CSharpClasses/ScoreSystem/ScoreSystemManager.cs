using Oculus.Platform.Samples.VrHoops;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Score system class. This class handles scoreing for all the players
//     individually and in teams. Score is increased when a projectile 
//     collisdes with an enemy.
// </summary>
//------------------------------------------------------------------------------
public class ScoreSystemManager : NetworkBehaviour
{
    public static ScoreSystemManager Singleton;
    private NetworkList<PlayerIndividualScore> scoreList;
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
        }
        else
        {
            Destroy(this);
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

    public void ScoreAddedToPlayer(ulong playerID, int scoreIncrease = 10)
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
        List<float> sortedScores = new List<float>();
        for (int i = 0; i < scoreList.Count; i++)
        {
            sortedScores.Add(scoreList[i].score);
        }

        //sortedScores.Sort();
        sortedScores.Sort((p1, p2) => p1.CompareTo(p2));

        //ClientRpc Update UI with leaderboard
    }
}
