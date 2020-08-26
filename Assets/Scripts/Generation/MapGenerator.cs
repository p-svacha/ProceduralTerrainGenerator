using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public DrawMode DrawMode;

    public const int MapChunkSize = 241; // 240 is divisible through 2,4,6,8,10
    [Range(0,6)]
    public int LevelOfDetail;
    [Range(10, 500)]
    public int HeightMultiplier;

    public Vector2 Offset;

    public bool autoUpdate;

    Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    public MapData GenerateMapData(float[,] worldHeightMap, Vector2 coordinates)
    {
        float[,] chunkHeightMap = new float[MapChunkSize, MapChunkSize];

        int worldWidth = worldHeightMap.GetLength(0);
        int worldHeight = worldHeightMap.GetLength(1);

        Color[] colorMap = new Color[MapChunkSize * MapChunkSize];
        for(int y = 0; y < MapChunkSize; y++)
        {
            for(int x = 0; x < MapChunkSize; x++)
            {
                // Get height value of current pixel in current chunk from world height map
                float currentHeight = worldHeightMap[(int)(coordinates.x * (MapChunkSize - 1)) + x, (worldHeight - MapChunkSize) - ((int)(coordinates.y * (MapChunkSize - 1))) + y];
                chunkHeightMap[x, y] = currentHeight;

                // Create color for this pixel
                int colorMapIndex = y * MapChunkSize + x;
                if (currentHeight == 0) colorMap[colorMapIndex] = Color.blue;
                else if (currentHeight <= 0.001f) colorMap[colorMapIndex] = Color.yellow;
                else if (currentHeight <= 0.6f) colorMap[colorMapIndex] = new Color(0, 0.5f, 0);
                else if (currentHeight <= 0.9f) colorMap[colorMapIndex] = Color.grey;
                else if (currentHeight <= 1f) colorMap[colorMapIndex] = Color.white;
            }
        }

        MapData mapData = new MapData(chunkHeightMap, colorMap);
        return mapData;
    }

    #region Threading
    public void RequestMapData(Action<MapData> callback, float[,] heightMap, Vector2 coordinates)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback, heightMap, coordinates);
        };
        new Thread(threadStart).Start();
    }

    // This function runs on a different thread
    public void MapDataThread(Action<MapData> callback, float[,] heightMap, Vector2 coordinates)
    {
        MapData mapData = GenerateMapData(heightMap, coordinates);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
        }
    }


    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    // This function runs on a different thread
    public void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData, HeightMultiplier, LevelOfDetail);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
        }
    }

    #endregion

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter);
            }
        }
    }

    #region editor
    public void DrawMapInEditor(MapData mapData)
    {
        MapDisplay display = GetComponent<MapDisplay>();
        display.DrawMapInEditor(DrawMode, MapChunkSize, mapData, HeightMultiplier, LevelOfDetail);
    }

    public void HideMapInEditor()
    {
        MapDisplay display = GetComponent<MapDisplay>();
        display.HideMap();
    }

    #endregion

    [System.Serializable]
    public struct TerrainType
    {
        public string Name;
        public float Height;
        public Color Color;
    }

    public struct ThreadInfo<T>
    {
        public readonly Action<T> Callback;
        public readonly T Parameter;

        public ThreadInfo(Action<T> callback, T parameter)
        {
            Callback = callback;
            Parameter = parameter;
        }
    }
}
