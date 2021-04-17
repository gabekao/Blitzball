using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GhostData
{
    public float[] playerPosition;
    public float[] playerRotation;

    public GhostData (PlayerController player)
    {
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;

        playerRotation = new float[3];
        playerRotation[0] = player.transform.transform.rotation.x;
        playerRotation[1] = player.transform.transform.rotation.y;
        playerRotation[2] = player.transform.transform.rotation.z;
    }
}
