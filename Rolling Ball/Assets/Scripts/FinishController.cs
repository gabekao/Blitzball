using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishController : MonoBehaviour
{
    private string levelName;
    private float timer;
    private int rank = 99;
    private bool newHighscore = false;
    public bool isFinished = false;

    // Starts timer for current level
    private void Start()
    {
        // Set the start of the timer
        //timer = Time.time.time;
    }
    
    // Checks if player collids with finish line
    private void OnTriggerEnter(Collider other)
    {
        // If collision object is player
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Collided");
            // Complete the level
            CompleteLevel();
            // Save the current run
            SaveRun();
        }
    }

    // Saves current run to file
    void SaveRun()
    {
        // Create file name for level
        levelName = "level_" + SceneManager.GetActiveScene().name;
        
        // Save data onto file

        if (newHighscore)
        {
            SaveData.current.ghostPositions.Clear();
            SaveData.current.currentPositions.ForEach(SaveData.current.ghostPositions.Add);
            //SaveData.current.ghostPositions = new List<GhostData>(SaveData.current.currentPositions);   // Remove old file???
        }
        Debug.Log(SaveData.current.lbData[0].name);
        SerializationManager.Save(levelName, SaveData.current);
    }

    // Performs all actions when level is completed
    void CompleteLevel()
    {
        // Attach new scores
        AddNewScore();

        // Set level to complete
        isFinished = true;
        // Level complete pop up

        // Advance to next level

    }

    // Adds a new score to leaderboard
    void AddNewScore()
    {
        // Checks to see if shift in leaderboard
        CompareScores();

        // Assign new leaderboard data
        LeaderboardData ldB = new LeaderboardData();

        // Values being attached
        ldB.name = "TMP";
        ldB.rank = rank;
        ldB.time = timer;

        // If the leaderboard is currently empty, add score automatically
        if (SaveData.current.lbData.Count < 1)
        {
            SaveData.current.lbData.Add(ldB);
            return;
        }

        // Adds new score information to data file
        for (int i = 0; i < SaveData.current.lbData.Count; i++)
        {
            Debug.Log("More than 1");

            // If current rank exceeds new incoming rank, add item
            if (SaveData.current.lbData[i].rank >= rank)
            {
                // If current rank is 5, remove and add new item
                if (SaveData.current.lbData[i].rank == 5)
                {
                    SaveData.current.lbData.RemoveAt(i);
                    SaveData.current.lbData.Add(ldB);
                }
                // If current rank is less than incoming rank, then increment current rank
                else
                    SaveData.current.lbData[i].rank += 1;
            }
        }

    }

    // Compares new score to current scores
    void CompareScores()
    {
        int i = 0;  // Incrementor

        // Find time difference
        timer = Time.timeSinceLevelLoad;//.time - timer;

        // Find the rank position based on new time
        for (i = 0; i < SaveData.current.lbData.Count; i++)
        {
            // If timer is less than current time and rank is greater than current rank
            if (SaveData.current.lbData[i].time > timer && SaveData.current.lbData[i].rank < rank)
            {
                // Assign current rank to rank
                rank = SaveData.current.lbData[i].rank;
            }
        }
        // If rank did not change, assign rank to incrementor + 1
        if (rank == 99)
            rank = i + 1;
        // If rank is 1, set newHighscore to true
        if (rank == 1)
            newHighscore = true;
        Debug.Log("rank = " + rank);
    }
}
