using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishController : MonoBehaviour
{
    [SerializeField] private GameObject finishPanel;        // Finish panel object
    [SerializeField] private TMPro.TMP_InputField input;    // Input field

    private MenuController menuController;      // Menu controller for pausing
    private SaveManager saveManager;            // Save Manager
    private SaveData newData;                   // New save data being worked with
    private SaveData loadedData;                // Loaded data being worked with
    private SaveData tmpData;                   // Temporary data file


    public bool isFinished = false;     // Checks if finish line was crossed


    private void Update()
    {
        // Check if objects are null, if so set values
        if (newData == null)
            newData = GameObject.Find("GameManager").GetComponent<GhostController>().newData;
        if (loadedData == null)
            loadedData = GameObject.Find("GameManager").GetComponent<GhostController>().loadedData;
        if (saveManager == null)
            saveManager = GameObject.Find("GameManager").GetComponent<SaveManager>();
        if (!finishPanel)
            finishPanel = GameObject.Find("FinishPanel");
        if (!menuController)
            menuController = GameObject.Find("MenuController").GetComponent<MenuController>();
        if (!isFinished)
            finishPanel.SetActive(false);
    }

    // Checks if player collids with finish line
    private void OnTriggerEnter(Collider other)
    {
        // If collision object is player
        if (other.gameObject.tag == "Player")
        {
            // Set track to finished
            isFinished = true;
            // Save the current run
            SaveGhost();
        }
    }

    // Saves current ghost data
    public void SaveGhost()
    {
        // Temporay save data to hold all new vallues
        SaveData g = new SaveData();

        // Add new save data to temp holder
        for (int i = 0; i < newData.positions.Count; i++)
        {
            g.positions.Add(newData.positions[i]);
        }

        for (int i = 0; i < newData.camPos.Count; i++)
        {
            g.camPos.Add(newData.camPos[i]);
        }

        // Set rank and time values
        g.rank = 1;
        g.time = menuController.timer;

        tmpData = g;

        // Pause game to save
        menuController.SimplePause();

        // Display Input name field;
        finishPanel.SetActive(true);

        // Enable cursor
        Cursor.lockState = CursorLockMode.None;
    }

    // Save data to file
    public void SaveRun()
    {
        // Create directory based on scene
        string directory = "/" + SceneManager.GetActiveScene().name + "/";

        // Initialize name
        string name;

        // Set name based on input text
        if (input.text.Length < 3)
            name = input.text;
        else
            name = input.text.Substring(0,3);

        // Set name
        tmpData.name = name;

        // Save tmpData
        SerializationManager.Save("tmp", directory, tmpData);

        saveManager.SortFiles();

        // Set finish panel to false
        finishPanel.SetActive(false);

        // Reload level or load next level
        menuController.OpenEndGamePanel();

        isFinished = false;
    }
}
