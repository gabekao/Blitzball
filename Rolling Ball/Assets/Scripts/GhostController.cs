using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GhostController : MonoBehaviour
{
    [SerializeField] private GameObject player;                 // Player object
    [SerializeField] private GameObject ghostPrefab;            // Ghost prefab object
    [SerializeField] private MenuController menuController;     // Contains pause state

    private GameObject finishLine;      // Finish line
    private GameObject ghost;           // Ghost object moving around
    private Transform initPos;          // Initial player position
    private SaveManager saveManager;    // Save manager being used

    public SaveData newData;            // New incoming save data
    public SaveData loadedData;         // Loaded data

    private int counter = 0;            // Ghost position counter
    private int frame = 1;              // For counting/limiting frames
    private bool ghostExists = false;   // Checks state of load file

    void Start()
    {
        // Attach player if null
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        // Attach ghost if null
        if (!ghostPrefab)
            ghostPrefab = GameObject.FindGameObjectWithTag("Ghost");

        // Attach finish line
        if (!finishLine)
            finishLine = GameObject.FindGameObjectWithTag("FinishLine");

        // Attach Menu Controller
        if (!menuController)
            menuController = GameObject.Find("MenuController").GetComponent<MenuController>();

        // Attach save manager
        if (!saveManager)
            saveManager = GameObject.Find("GameManager").GetComponent<SaveManager>();

        // Initial player transform
        initPos = player.transform;

        // Initialize save data objects
        newData = new SaveData();
        loadedData = new SaveData();

        // Check to see if load file available
        CheckGhost();
    }


    // Update is called once per frame
    void Update()
    {
        if (!menuController.isPaused)
        {
            Debug.Log("Counter: " + counter);
            // Perform action every 3 frames while not finished
            if ((frame % 4) == 0 && !finishLine.GetComponent<FinishController>().isFinished)
            {
                // Load ghost if load file exists
                if (ghostExists)
                    LoadGhost();
                // Save current player position
                SaveGhost();
                // Reset frame counter to 1
                frame = 1;
            }
            // Increment frame counter
            frame++;
        }
    }

    // Saves player positions as ghost data
    public void SaveGhost()
    {
        // Create new ghost data object containing player positions/rotation
        GhostData gD = new GhostData
        {
            position = player.transform.position,
            rotation = player.transform.rotation
        };
        // Add ghost data to current positions list
        newData.positions.Add(gD);
    }

    // Loads ghost positional data
    public void LoadGhost()
    {
        // Create ghost object to obtain ghost data from load file
        GhostData pos = new GhostData();
        pos = loadedData.positions[counter];

        // Move created ghost object by positional values
        ghost.transform.position = pos.position;
        ghost.transform.rotation = pos.rotation;
        
        // Increment position counter until it reaches the end
        if (loadedData.positions.Count - 1 != counter)
            counter++;
    }

    // Check to see if load data is available
    public void CheckGhost()
    {
        // Get directory name
        string directory = "/" + SceneManager.GetActiveScene().name + "/";

        // Read 1st save data
        loadedData = (SaveData)SerializationManager.Load(Application.persistentDataPath + directory + "1.data");

        // Check if ghost positions are available
        if (loadedData != null)
        {

            // Set ghost exists to true
            ghostExists = true;

            // Create ghost object starting at player position
            if (ghost == null)
                ghost = Instantiate(ghostPrefab, initPos.position, Quaternion.identity);

            // Set counter to 0
            counter = 0;

            // Display ghost time
            saveManager.DisplayTime(loadedData.time);
        }
    }

    // Loads new ghost data
    public void LoadNewGhost(SaveData g)
    {
        // Set new loaded data
        loadedData = g;

        // Ensure ghost exists
        ghostExists = true;

        // Initialize ghost position, counter and frame
        ghost.transform.position = initPos.position;
        ghost.transform.rotation = initPos.rotation;
        counter = 0;
        frame = 1;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // Attach player if null
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player");

            // Attach ghost if null
            if (!ghostPrefab)
                ghostPrefab = GameObject.FindGameObjectWithTag("Ghost");

            // Attach finish line
            if (!finishLine)
                finishLine = GameObject.FindGameObjectWithTag("FinishLine");

            // Attach Menu Controller
            if (!menuController)
                menuController = GameObject.Find("MenuController").GetComponent<MenuController>();

            // Attach save manager
            if (!saveManager)
                saveManager = GameObject.Find("GameManager").GetComponent<SaveManager>();

            // Initial player transform
            initPos = player.transform;

            // Initialize save data objects
            newData = new SaveData();
            loadedData = new SaveData();

            // Check to see if load file available
            CheckGhost();
        }
    }
}
