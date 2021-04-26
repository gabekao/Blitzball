using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostController : MonoBehaviour
{
    [SerializeField] GameObject player;             // Player object
    [SerializeField] GameObject ghostPrefab;        // Ghost prefab object

    GameObject finishLine;  // Finish line
    GameObject ghost;       // Ghost object moving around

    int counter = 0;            // Ghost position counter
    int frame = 1;              // For counting/limiting frames
    bool ghostExists = false;   // Checks state of load file

    // Start is called before the first frame update
    void Start()
    {
        // Attach player if null
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        // Attach ghost if null
        if (!ghostPrefab)
            ghostPrefab = GameObject.FindGameObjectWithTag("Ghost");

        if (!finishLine)
            finishLine = GameObject.FindGameObjectWithTag("FinishLine");
        // Check to see if load file available
        CheckGhost();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            CheckGhost();
        }
        if (!finishLine)
            finishLine = GameObject.FindGameObjectWithTag("FinishLine");

        Debug.Log(finishLine.GetComponent<FinishController>().isFinished);
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
        //Debug.Log(Time.timeSinceLevelLoad);
    }

    // Saves player positions as ghost data
    public void SaveGhost()
    {
        // Create new ghost data object containing player positions/rotation
        GhostData ghostData = new GhostData
        {
            position = player.transform.position,
            rotation = player.transform.rotation
        };
        // Add ghost data to current positions list
        SaveData.current.currentPositions.Add(ghostData);
    }

    // Loads ghost positional data
    public void LoadGhost()
    {
        // Create ghost object to obtain ghost data from load file
        GhostData pos = new GhostData();
        pos = SaveData.current.ghostPositions[counter];

        // Move created ghost object by positional values
        ghost.transform.position = pos.position;
        ghost.transform.rotation = pos.rotation;
        
        // Increment position counter until it reaches the end
        if (SaveData.current.ghostPositions.Count - 1 != counter)
            counter++;
    }

    // Check to see if load data is available
    public void CheckGhost()
    {
        // Read save data
        SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/ghost_" + SceneManager.GetActiveScene().name + ".data");
        // Clear current positions
        //SaveData.current.currentPositions.Clear();

        // Check if ghost positions are available
        if (SaveData.current.ghostPositions.Count > 0)
        {
            // Set ghost exists to true
            ghostExists = true;
            // Create ghost object starting at player position
            ghost = Instantiate(ghostPrefab, player.transform.position, Quaternion.identity);
        }
    }
}
