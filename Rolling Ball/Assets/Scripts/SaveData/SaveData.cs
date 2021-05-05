using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Contains player information
    public PlayerProfile profile = new PlayerProfile();

    // Contains previously mapped positions, if they exist
    public List<GhostData> positions = new List<GhostData>();

    // Contains camera positions
    public List<CameraData> camPos = new List<CameraData>();

    // Contains rank
    public int rank;

    // Contains assigned name
    public string name;

    // Contains time
    public float time;
}
