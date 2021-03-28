using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] private GameObject menuPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "StartMenu")
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
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
