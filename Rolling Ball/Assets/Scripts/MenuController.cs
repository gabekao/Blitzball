using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //🔴 Lmao this method is so scuffed. Sorry for the ugliness.
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject[] ranks;
    private bool isPaused = false;
    private bool isLeaderboard = false;

    public void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "StartMenu")
        {
            if (!isLeaderboard)
            {
                if (!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
            else
                ExitLeaderboard();
        }
    }

    public void LoadStartMenu()
    {
        //SceneManager.LoadScene("StartMenu");
        SceneManager.LoadScene(0);
        if (isPaused)
            Time.timeScale = 1;
    }

    public void LoadGame()
    {
        //SceneManager.LoadScene("Level 1");
        SceneManager.LoadScene(1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (isPaused)
            ResumeGame();
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void OpenLeaderBoard()
    {
        menuPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        PopulateLeaderboard();
        isLeaderboard = true;
    }

    public void ExitLeaderboard()
    {
        leaderboardPanel.SetActive(false);
        menuPanel.SetActive(true);
        isLeaderboard = false;
    }

    public void PopulateLeaderboard()
    {
        int count = SaveData.current.lbData.Count;
        string date = System.DateTime.Now.ToString("MM/dd/yyyy");

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                System.TimeSpan time = System.TimeSpan.FromSeconds(SaveData.current.lbData[i].time);
                string t = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds);
                string name = SaveData.current.lbData[i].name;
                int rank = SaveData.current.lbData[i].rank;
                ranks[rank - 1].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", rank, name, t, date);
            }
        }
        if (count < 5)
            FillEmptySpots(count);
    }


    void FillEmptySpots(int filled)
    {
        for (int i = filled; i < ranks.Length; i++)
        {
            ranks[i].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", (i + 1).ToString(), "---", "00:00:000", "00/00/0000");
        }
    }

    // Game is now over
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
