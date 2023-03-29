using Oculus.Platform.Samples.VrHoops;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------
// </summary>
//     Score system class. This class handles scoreing for all the players
//     individually and in teams. Score is increased when a projectile 
//     collisdes with an enemy.
// </summary>
//------------------------------------------------------------------------------
public class ScoreSystemManager : MonoBehaviour
{
    public static ScoreSystemManager Instance;
    private List<PlayerIndividualScore> scoreList = new List<PlayerIndividualScore>();
    public class PlayerIndividualScore
    {
        public int playerID; //change to network connection
        public float score;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        NewPlayerConnected(0);
    }

    public void NewPlayerConnected(int playerID)
    {
        PlayerIndividualScore playerIndividualScore = new PlayerIndividualScore();
        playerIndividualScore.playerID = playerID;
        playerIndividualScore.score = 0;
        scoreList.Add(playerIndividualScore);
    }

    public void PlayerDisonnected(int playerID)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == playerID)
            {
                scoreList.RemoveAt(i);
                return;
            }
        }
    }

    public void ScoreAddedToPlayer(int playerID, int scoreIncrease = 10)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].playerID == playerID)
            {
                scoreList[i].score += scoreIncrease;
                return;
            }
        }
    }


}
