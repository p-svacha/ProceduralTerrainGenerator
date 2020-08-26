using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData
{
    public readonly float[,] HeightMap;
    public readonly Color[] ColorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        HeightMap = heightMap;
        ColorMap = colorMap;
    }
}
