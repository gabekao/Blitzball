using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Constructing SaveData object
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            // If null, create new SaveData
            if (_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
        set
        {
            // If value passed is not null, set SaveData to value
            if (value != null)
            {
                _current = value;
            }
        }
    }

    // Contains player information
    public PlayerProfile profile = new PlayerProfile();

    // Records current positions being mapped
    public List<GhostData> currentPositions = new List<GhostData>();

    // Contains previously mapped positions, if they exist
    public List<GhostData> ghostPositions = new List<GhostData>();
}
