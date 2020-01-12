using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChunk
{
    public InfiniteTerrain InfiniteTerrain;

    // Attributes
    public GameObject MeshObject;
    public int Size;
    public MeshData MeshData;
    public Vector2 Coordinates;
    public Vector2 Position;
    public Bounds Bounds;

    // Components
    MeshFilter MeshFilter;
    MeshRenderer MeshRenderer;

    public MapChunk(Vector2 coordinates, int size, InfiniteTerrain infTerrain, Material material)
    {
        
        Size = size;
        Coordinates = coordinates;
        Position = coordinates * size;
        InfiniteTerrain = infTerrain;

        Bounds = new Bounds(Position, Vector2.one * Size);

        MeshObject = new GameObject("Map Chunk");
        MeshObject.transform.parent = InfiniteTerrain.transform;
        MeshObject.transform.position = new Vector3(Position.x, 0, Position.y);

        MeshRenderer = MeshObject.AddComponent<MeshRenderer>();
        MeshFilter = MeshObject.AddComponent<MeshFilter>();

        MeshRenderer.material = material;

        SetVisible(false);

        InfiniteTerrain.MapGenerator.RequestMapData(OnMapDataReceived);
    }

    void OnMapDataReceived(MapData mapData)
    {
        InfiniteTerrain.MapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
    }

    void OnMeshDataReceived(MeshData meshData)
    {
        MeshFilter.mesh = meshData.CreateMesh();
    }

    public void UpdateChunk()
    {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(Bounds.SqrDistance(InfiniteTerrain.ViewerPosition));
        bool visible = viewerDistanceFromNearestEdge < InfiniteTerrain.MaxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        MeshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return MeshObject.activeSelf;
    }
}
