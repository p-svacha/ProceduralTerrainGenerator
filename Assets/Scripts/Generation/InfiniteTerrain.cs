using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    
    public MapGenerator MapGenerator;

    public const float MaxViewDistance = 660;

    public Material MapMaterial;
    public Transform Viewer;

    public static Vector2 ViewerPosition;

    public int ChunkSize;
    public int ChunksVisibleInViewDistance;

    // Generated Chunks
    Dictionary<Vector2, MapChunk> MapChunkDictionary = new Dictionary<Vector2, MapChunk>();
    List<MapChunk> mapChunksVisibleLastUpdate = new List<MapChunk>();

    void Start()
    {
        MapGenerator = FindObjectOfType<MapGenerator>();
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
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if(MapChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    MapChunk loadedChunk = MapChunkDictionary[viewedChunkCoord];
                    loadedChunk.UpdateChunk();
                    if (loadedChunk.IsVisible()) mapChunksVisibleLastUpdate.Add(loadedChunk);
                }
                else
                {
                    MapChunk newChunk = new MapChunk(viewedChunkCoord, ChunkSize, this, MapMaterial);
                    MapChunkDictionary.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }
}
