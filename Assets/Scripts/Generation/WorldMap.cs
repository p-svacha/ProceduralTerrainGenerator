using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public RandomNumberGenerator RNG;
    public MapGenerator MapGenerator;
    public int ChunkSize;

    // Map Generation Attributes
    public int Seed;
    public int WorldMapWidth; // in # chunks
    public int WorldMapHeight; // in # chunks

    public float NoiseScale;

    [Range(1, 8)]
    public int Octaves;
    [Range(0, 1)]
    public float Persistance; // 0.5
    [Range(1, 4)]
    public float Lacunarity; // 2

    [Range(0, 1)]
    public float SeaLevel;

    public HeightCurveOperation HeightCurveOperation;
    public bool DynamicHeightExtremes;
    public float MinNoiseHeight;
    public float MaxNoiseHeight;

    // View
    public const float MaxViewDistance = 5000;
    public Material MapMaterial;
    public Transform Viewer;
    public static Vector2 ViewerPosition;
    public int ChunksVisibleInViewDistance;

    // Map Data
    public float[,] HeightMap;
    private Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> mapChunksVisibleLastUpdate = new List<TerrainChunk>();

    // Editor
    public bool AutoUpdate;

    void Start()
    {
        MapGenerator = GetComponent<MapGenerator>();

        // Initialize random number generator
        RNG = new RandomNumberGenerator(Seed);

        // Create heightmap for whole map
        HeightMap = HeightMapGenerator.GenerateNoiseMapWithUnityPerlin(MapGenerator.MapChunkSize, WorldMapWidth, WorldMapHeight, RNG, NoiseScale, Octaves, Persistance, Lacunarity, SeaLevel, HeightCurveOperation, DynamicHeightExtremes, MinNoiseHeight, MaxNoiseHeight);
        //HeightMap = HeightMapGenerator.GenerateHeightMapWithFoldAlgorithm(MapGenerator.MapChunkSize, this, RNG);

        ChunkSize = MapGenerator.MapChunkSize - 1;
        ChunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / ChunkSize);
    }

    void Update()
    {
        ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);
        UpdateVisibleChunks();
    }

  

    void UpdateVisibleChunks()
    {
        // Set previously visible chunks invisible
        for(int i = 0; i < mapChunksVisibleLastUpdate.Count; i++)
        {
            mapChunksVisibleLastUpdate[i].SetVisible(false);
        }
        mapChunksVisibleLastUpdate.Clear();

        // Set now visible chunks visible
        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

        for(int yOffset = -ChunksVisibleInViewDistance; yOffset <= ChunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -ChunksVisibleInViewDistance; xOffset <= ChunksVisibleInViewDistance; xOffset++)
            {
                // Coordinates of current chunk in loop
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                // Only load chunk if it is in world map bounds
                if (viewedChunkCoord.x >= 0 && viewedChunkCoord.x < WorldMapWidth && viewedChunkCoord.y >= 0 && viewedChunkCoord.y < WorldMapHeight)
                {
                    if (TerrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        TerrainChunk loadedChunk = TerrainChunkDictionary[viewedChunkCoord];
                        loadedChunk.UpdateChunk();
                        if (loadedChunk.IsVisible()) mapChunksVisibleLastUpdate.Add(loadedChunk);
                    }
                    else
                    {
                        GameObject newTerrainChunkObject = new GameObject();
                        TerrainChunk newTerrainChunk = newTerrainChunkObject.AddComponent<TerrainChunk>();
                        newTerrainChunk.Initialize(viewedChunkCoord, ChunkSize, this, MapMaterial);
                        TerrainChunkDictionary.Add(viewedChunkCoord, newTerrainChunk);
                    }
                }
            }
        }
    }

    void OnValidate()
    {
        if (Seed < 1) Seed = 1;
    }
}
