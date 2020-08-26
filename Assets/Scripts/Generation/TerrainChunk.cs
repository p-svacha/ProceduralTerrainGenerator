using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public WorldMap WorldMap;

    // Attributes
    public int Size;
    public MeshData MeshData;
    public Vector2 Coordinates;
    public Vector2 Position;
    public Bounds Bounds;

    // Components
    MeshFilter MeshFilter;
    MeshRenderer MeshRenderer;

    // Should be called right after being instantiated
    public void Initialize(Vector2 coordinates, int size, WorldMap infTerrain, Material material)
    {
        Size = size;
        Coordinates = coordinates;
        Position = coordinates * size;
        WorldMap = infTerrain;

        Bounds = new Bounds(Position, Vector2.one * Size);

        gameObject.name = "TerrainChunk " + Coordinates.x + "/" + Coordinates.y;
        gameObject.transform.parent = WorldMap.transform;
        gameObject.transform.position = new Vector3(Position.x, 0, Position.y);

        MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer.material = material;

        SetVisible(false);

        WorldMap.MapGenerator.RequestMapData(OnMapDataReceived, WorldMap.HeightMap, Coordinates);
    }

    void OnMapDataReceived(MapData mapData)
    {
        WorldMap.MapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
        MeshRenderer.material.mainTexture = texture;
    }

    void OnMeshDataReceived(MeshData meshData)
    {
        MeshFilter.mesh = meshData.CreateMesh();
    }

    public void UpdateChunk()
    {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(Bounds.SqrDistance(WorldMap.ViewerPosition));
        bool visible = viewerDistanceFromNearestEdge < WorldMap.MaxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }
}
