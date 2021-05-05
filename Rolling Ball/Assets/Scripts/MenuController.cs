﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    //🔴 Lmao this method is so scuffed. Sorry for the ugliness.
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

        if(mixer){
            mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVol")) * 20);
            mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVol")) * 20);
        }
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
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
        if (Input.GetKeyUp(KeyCode.L)){
            LoadNextLevel();
        }
        if (!start)
            StartTimer();
        else
            DisplayTime();

        if (SceneManager.GetActiveScene().name != "StartMenu"  )
        {
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

    public void StartTimer()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetButton("Jump"))
            start = true;
        timeOffset = Time.timeSinceLevelLoad;
    }

    public void DisplayTime()
    {
        timer = Time.timeSinceLevelLoad - timeOffset;
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
        string t;

        if (timer > 60)
            t = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds/10);
        else
            t = string.Format("{0:D2}:{1:D2}", timeSpan.Seconds, timeSpan.Milliseconds/10);

        timerText.text = t;
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

    public void LoadNextLevel()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void SimplePause()
    {
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

    public void OpenEndGamePanel()
    {
        endGamePanel.SetActive(true);
    }

    public void PopulateLeaderboard()
    {
        saveManager.SortFiles();

        int count = saveManager.saveFiles.Length;
        
        if (count > 0)
        {
            if (count > 5)
                count = 5;

            string date = System.DateTime.Now.ToString("MM/dd/yyyy");

            for (int i = 0; i < count; i++)
            {
                SaveData tmp = (SaveData)SerializationManager.Load(saveManager.saveFiles[i]);

                System.TimeSpan time = System.TimeSpan.FromSeconds(tmp.time);
                string t = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds/10);
                string name = tmp.name;
                int rank = tmp.rank;
                ranks[rank - 1].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", rank, name, t, date);
            }
        }
        if (count < 5)
            FillEmptySpots(count);
    }


    // Fills in empty spots on the leader board
    void FillEmptySpots(int filled)
    {
        // start
        for (int i = filled; i < ranks.Length; i++)
        {
            ranks[i].GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0, 1}\t\t{1, 3}\t\t{2, 9}\t\t{3, 10}", (i + 1).ToString(), "---", "00:00:000", "00/00/0000");
        }
    }

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
