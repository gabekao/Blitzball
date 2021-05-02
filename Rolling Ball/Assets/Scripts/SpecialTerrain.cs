using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialTerrainType
{
    None,
    Sand,
    Rubber,
    Slippery
}


public class SpecialTerrain : MonoBehaviour
{
    public SpecialTerrainType terrainType;
}
