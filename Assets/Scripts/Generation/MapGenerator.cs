using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public int MapWidth;
    public int MapHeight;
    public float NoiseScale;

    [Range(1,8)]
    public int Octaves;
    [Range(0,1)]
    public float Persistance; // 0.5
    public float Lacunarity; // 2

    public int Seed;
    public Vector2 Offset;

    public bool autoUpdate;

    public void GenerateMap()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator(Seed);
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(MapWidth, MapHeight, RNG, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

    void OnValidate()
    {
        if (MapWidth < 1) MapWidth = 1;
        if (MapHeight < 1) MapHeight = 1;
        if (Lacunarity < 1) Lacunarity = 1;
    }
}
