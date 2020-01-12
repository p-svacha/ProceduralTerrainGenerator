using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    RandomNumberGenerator RNG;
    public DrawMode DrawMode;

    public const int MapChunkSize = 241; // 240 is divisible through 2,4,6,8,10
    [Range(0,6)]
    public int LevelOfDetail;

    public float NoiseScale;
    [Range(10,50)]
    public float HeightMultiplier;

    [Range(1,8)]
    public int Octaves;
    [Range(0,1)]
    public float Persistance; // 0.5
    [Range(1,4)]
    public float Lacunarity; // 2

    public int Seed;
    public Vector2 Offset;

    public bool autoUpdate;

    public TerrainType[] Regions;

    Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    void Start()
    {
        RNG = new RandomNumberGenerator(Seed);
    }


    public MapData GenerateMapData()
    {
        if(RNG == null) RNG = new RandomNumberGenerator(Seed); // used for editor
        RNG = new RandomNumberGenerator(Seed);
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(MapChunkSize, RNG, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

        // Apply region colors to noise map
        Color[] colorMap = new Color[MapChunkSize * MapChunkSize];
        for(int y = 0; y < MapChunkSize; y++)
        {
            for(int x = 0; x < MapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for(int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight <= Regions[i].Height)
                    {
                        colorMap[y * MapChunkSize + x] = Regions[i].Color;
                        break;
                    }
                }
            }
        }

        MapData mapData = new MapData(noiseMap, colorMap);
        return mapData;
    }

    #region Threading
    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }

    // This function runs on a different thread
    public void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
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

    void OnValidate()
    {
        if (Seed < 1) Seed = 1;
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
