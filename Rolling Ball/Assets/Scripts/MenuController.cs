using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    //🔴 Lmao this method is so scuffed. Sorry for the ugliness.
    [SerializeField] private GameObject Level2Final;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject[] ranks;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    private SaveManager saveManager;
    private GhostController ghostController;
    private FinishController finishController;
    private SaveData loadedData;

    private bool isLeaderboard = false;
    private float timeOffset;
    public float timer;
    public bool isPaused = false;
    public bool start = false;

    //Audio Start
    [SerializeField] AudioMixer mixer;
    //Audio End

    public void Start()
    {

        // Mixer handling if it exists
        if(mixer){
            mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVol")) * 20);
            mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVol")) * 20);
        }

        // Check to see that game is not in start menu
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // Ensure all objects are loaded if null
            if (!menuPanel)
                menuPanel = GameObject.Find("MenuPanel");

            if (!leaderboardPanel)
                leaderboardPanel = GameObject.Find("LeaderBoardPanel");

            if (!timerPanel)
                timerPanel = GameObject.Find("TimerPanel");

            if (!endGamePanel)
                endGamePanel = GameObject.Find("EndGamePanel");
            
            if (loadedData == null)
                loadedData = GameObject.Find("GameManager").GetComponent<GhostController>().loadedData;

            if (saveManager == null)
                saveManager = GameObject.Find("GameManager").GetComponent<SaveManager>();

            if (ghostController == null)
                ghostController = GameObject.Find("GameManager").GetComponent<GhostController>();

            if (finishController == null)
                finishController = GameObject.Find("FinishLine").transform.GetChild(0).GetComponent<FinishController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Loads the next scene incrementally
        if (Input.GetKeyUp(KeyCode.L)){
            LoadNextLevel();
        }

        // Handles when the timer starts
        if (!start)
            StartTimer();
        else
            DisplayTime();

        // If not currently in start menu, perform appropriate GUI functions
        if (SceneManager.GetActiveScene().name != "StartMenu"  )
        {
            // ESC for leaderboard + pause panel
            if (Input.GetKeyDown(KeyCode.Escape))
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

            // If game is not paused lock cursor, else unlock
            if (!isPaused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if(start)
                    timerPanel.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                if(start)
                    timerPanel.SetActive(false);
            }
        }
    }

    // Starts the timer from when the player begins to move
    public void StartTimer()
    {
        // Check for any player input, set start to true when moving
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetButton("Jump"))
            start = true;
        timeOffset = Time.timeSinceLevelLoad;
    }

    // Display current time on the timer panel text box
    public void DisplayTime()
    {
        // Calculate time
        timer = Time.timeSinceLevelLoad - timeOffset;

        // Use timespan for formatting
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
        string t;

        // Grab string from timespan
        if (timer > 60)
            t = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds/10);
        else
            t = string.Format("{0:D2}:{1:D2}", timeSpan.Seconds, timeSpan.Milliseconds/10);

        // Set timer onto text
        timerText.text = t;
    }

    // Loads the start menu scene
    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);

        // If game was paused set time scale back to 1
        if (isPaused)
            Time.timeScale = 1;
    }

    // Loads the first level of the game
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    // Loads the next level of the game
    public void LoadNextLevel()
    {
        // If currently on level 2, present the end of demo panel, else increment scene
        if (SceneManager.GetActiveScene().name == "Level 2" && Level2Final){
            Time.timeScale = 0;
            isPaused = true;
            Level2Final.SetActive(true);
        } else {
            isPaused = false;
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Reloads current level
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // If game was paused, ensure to resume
        if (isPaused)
            ResumeGame();
    }

    // Pauses game, prompting menu panel
    public void PauseGame()
    {
        // Set menu panel to true
        menuPanel.SetActive(true);
        // Paused state is now tru
        isPaused = true;
        // Time scale is 0
        Time.timeScale = 0;
    }

    // Simple pause, pauses the game without prompting menu panel
    public void SimplePause()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    // Resumes game, disabling the menu panel
    public void ResumeGame()
    {
        // Menu panel is disabled
        menuPanel.SetActive(false);
        // Game state is no longer paused
        isPaused = false;
        // Time scale back to normal
        Time.timeScale = 1;
    }

    // Opens leaderboard panel
    public void OpenLeaderBoard()
    {
        // Disable menu panel
        menuPanel.SetActive(false);
        // Enable leaderboard panel
        leaderboardPanel.SetActive(true);
        // Populates empty leaderboard slots
        PopulateLeaderboard();
        // Sets leaderboard state to true
        isLeaderboard = true;
    }

    // Closes opened leaderboard
    public void ExitLeaderboard()
    {
        leaderboardPanel.SetActive(false);
        menuPanel.SetActive(true);
        isLeaderboard = false;
    }

    // Opens the endgame panel 
    public void OpenEndGamePanel()
    {
        endGamePanel.SetActive(true);
    }

    // Fills out the leaderboard based on data files if available
    public void PopulateLeaderboard()
    {
        // Sort files beforehand
        saveManager.SortFiles();

        // Grab the amount of files being used
        int count = saveManager.saveFiles.Length;
        
        // If more than 0, load files
        if (count > 0)
        {
            // Set limit to 5
            if (count > 5)
                count = 5;

            // Grab current date
            string date = System.DateTime.Now.ToString("MM/dd/yyyy");

            // Loop until all files are read
            for (int i = 0; i < count; i++)
            {
                // Load current file
                SaveData tmp = (SaveData)SerializationManager.Load(saveManager.saveFiles[i]);

                // Grab time, name and rank
                System.TimeSpan time = System.TimeSpan.FromSeconds(tmp.time);
                string t = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds/10);
                string name = tmp.name;
                int rank = tmp.rank;

                // Add string onto the text component
                ranks[rank - 1].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", rank, name, t, date);
            }
        }
        // If count is still less than 5, fill remaining spots
        if (count < 5)
            FillEmptySpots(count);
    }


    // Fills in empty spots on the leader board
    void FillEmptySpots(int filled)
    {
        // Starting from filled increment until all spots are full
        for (int i = filled; i < ranks.Length; i++)
        {
            ranks[i].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", (i + 1).ToString(), "---", "00:00:000", "00/00/0000");
        }
    }

    // Selects a new ghost for the system to load
    public void SelectNewGhost()
    {
        // Get directory name
        string directory = "/" + SceneManager.GetActiveScene().name + "/";

        // Get button name
        string t = EventSystem.current.currentSelectedGameObject.name;

        // Exit leaderboard
        ExitLeaderboard();

        // Resume Game
        ResumeGame();

        // Set start to false;
        start = false;

        // Set Current ghost
        PlayerPrefs.SetInt("CurrentGhost", System.Int32.Parse(t));

        // Move player to start
        ReloadLevel();
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
