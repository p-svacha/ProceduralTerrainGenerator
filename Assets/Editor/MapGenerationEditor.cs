using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MapGenerator;

/// <summary>
/// Custom Editor for MapGenerator Script
/// </summary>
[CustomEditor (typeof (WorldMap))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldMap worldMap = (WorldMap)target;

        // Every time a value is changed and autoUpdate is selected, update the map
        if( DrawDefaultInspector())
        {
            if(worldMap.AutoUpdate && worldMap.WorldMapWidth <= 10 && worldMap.WorldMapHeight <= 10)
            {
                worldMap.RNG = new RandomNumberGenerator(worldMap.Seed);
                //float[,] heightMap = HeightMapGenerator.GenerateNoiseMapWithUnityPerlin(MapGenerator.MapChunkSize, worldMap.WorldMapWidth, worldMap.WorldMapHeight, worldMap.RNG, worldMap.NoiseScale, worldMap.Octaves, worldMap.Persistance, worldMap.Lacunarity, worldMap.SeaLevel, worldMap.HeightCurveOperation, worldMap.DynamicHeightExtremes, worldMap.MinNoiseHeight, worldMap.MaxNoiseHeight);
                float[,] heightMap = HeightMapGenerator.GenerateHeightMapWithOwnAlgorithm(MapGenerator.MapChunkSize, worldMap, worldMap.RNG);
                worldMap.GetComponent<MapDisplay>().DrawHeightMap(heightMap);
            }
        }


        // Add a Generate button that generates a new map with the current values
        if (GUILayout.Button("Show Height Map"))
        {
            worldMap.RNG = new RandomNumberGenerator(worldMap.Seed);
            //float[,] heightMap = HeightMapGenerator.GenerateNoiseMapWithUnityPerlin(MapGenerator.MapChunkSize, worldMap.WorldMapWidth, worldMap.WorldMapHeight, worldMap.RNG, worldMap.NoiseScale, worldMap.Octaves, worldMap.Persistance, worldMap.Lacunarity, worldMap.SeaLevel, worldMap.HeightCurveOperation, worldMap.DynamicHeightExtremes, worldMap.MinNoiseHeight, worldMap.MaxNoiseHeight);
            float[,] heightMap = HeightMapGenerator.GenerateHeightMapWithOwnAlgorithm(MapGenerator.MapChunkSize, worldMap, worldMap.RNG);
            worldMap.GetComponent<MapDisplay>().DrawHeightMap(heightMap);
        }
        
    }

}
