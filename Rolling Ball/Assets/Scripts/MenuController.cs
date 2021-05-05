using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //🔴 Lmao this method is so scuffed. Sorry for the ugliness.
    public bool isPaused = false;
    [SerializeField] private GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Changing this line since this script won't exist in the start menu.
        // if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "StartMenu")
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused){
                PauseGame();
            }
            else{
                ResumeGame();
            }
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
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        isPaused = false;
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
