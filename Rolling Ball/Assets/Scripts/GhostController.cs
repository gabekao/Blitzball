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
    [SerializeField] private RawImage ghostCam;                 // Ghost camera

    private GameObject finishLine;      // Finish line
    private GameObject ghost;           // Ghost object moving around
    private GameObject ghostCamera;     // Ghost camera
    private GameObject mainCamera;      // Main camera
    private Transform initPos;          // Initial player position
    private Transform initGhost;        // Initial ghost position
    private SaveManager saveManager;    // Save manager being used

    public SaveData newData;            // New incoming save data
    public SaveData loadedData;         // Loaded data

    private int counter = 0;            // Ghost position counter
    private int camCounter = 0;         // Camera position counter
    private int frame = 1;              // For counting/limiting frames
    private int curGhost = 1;           // Current leaderboard ghost
    private bool ghostExists = false;   // Checks state of load file


    void Start()
    {
        // Attach player if null
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        // Attach camera if null
        if (!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        // Attach camera if null
        if (!ghostCamera)
            ghostCamera = GameObject.FindGameObjectWithTag("GhostCamera");

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
        
        ghostExists = false;

        initGhost = ghostCamera.transform;

        // Check to see if load file available
        CheckGhost();
    }


    private void FixedUpdate()
    {
        if (!menuController.isPaused && menuController.start)
        {
            // Perform action every 3 frames while not finished
            if (!finishLine.GetComponent<FinishController>().isFinished)
            {
                // Load ghost if load file exists
                if (ghostExists)
                {
                    LoadGhost();
                    //LoadCamera();
                }
                // Save current player position
                SaveGhost();
                SaveCamera();
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

    public void SaveCamera()
    {
        CameraData cD = new CameraData
        {
            position = mainCamera.transform.position,
            rotation = mainCamera.transform.rotation
        };

        newData.camPos.Add(cD);
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

        // Create ghost camera data from loaded data
        CameraData cam = new CameraData();
        cam = loadedData.camPos[counter];

        // Set ghost camera position
        ghostCamera.transform.position = cam.position;
        //ghostCamera.transform.rotation = cam.rotation;
        ghostCamera.transform.LookAt(ghost.transform);


        // Increment position counter until it reaches the end
        if (loadedData.positions.Count - 1 != counter)
            counter++;
    }

    public void LoadCamera()
    {
        // Create ghost camera data from loaded data
        CameraData cam = new CameraData();
        cam = loadedData.camPos[camCounter];

        // Set ghost camera position
        ghostCamera.transform.position = cam.position;
        //ghostCamera.transform.rotation = cam.rotation;
        ghostCamera.transform.LookAt(ghost.transform);

        if (loadedData.camPos.Count - 1 != camCounter)
            camCounter++;
    }

    // Check to see if load data is available
    public void CheckGhost()
    {
        // Get ghost prefs
        curGhost = PlayerPrefs.GetInt("CurrentGhost");

        // If curGhost is not in range, set to 1
        if (curGhost < 1 || curGhost > 5)
            curGhost = 1;

        // Get directory name
        string directory = "/" + SceneManager.GetActiveScene().name + "/";

        // Read 1st save data
        loadedData = (SaveData)SerializationManager.Load(Application.persistentDataPath + directory + curGhost.ToString() + ".data");

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
            camCounter = 0;
        }

        if (!ghostExists)
            ghostCamera.SetActive(false);
        else
            ghostCamera.SetActive(true);

        ghostCamera.transform.position = initGhost.position;
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

            // Attach camera if null
            if (!mainCamera)
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            // Attach camera if null
            if (!ghostCamera)
                ghostCamera = GameObject.FindGameObjectWithTag("GhostCamera");

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

            ghostExists = false;

            initGhost = ghostCamera.transform;

            // Check to see if load file available
            CheckGhost();
        }
    }
}
