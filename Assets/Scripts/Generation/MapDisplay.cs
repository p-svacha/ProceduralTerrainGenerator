using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class MapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    public void DrawHeightMap(float[,] heightMap)
    {
        Texture2D heightMapTexture = TextureGenerator.TextureFromHeightMap(heightMap);
        MeshRenderer.gameObject.SetActive(false);
        TextureRenderer.gameObject.SetActive(true);
        DrawTexture(heightMapTexture);
    }

    public void DrawMapInEditor(DrawMode drawMode, int mapChunkSize, MapData mapData, float heightMultiplier, int levelOfDetail)
    {
        // Create Textures
        Texture2D heightMapTexture = TextureGenerator.TextureFromHeightMap(mapData.HeightMap);
        Texture2D colorMapTexture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, mapChunkSize, mapChunkSize);

        // Create Mesh
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData, heightMultiplier, levelOfDetail);

        // Draw Map according to DrawMode

        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                MeshRenderer.gameObject.SetActive(false);
                TextureRenderer.gameObject.SetActive(true);
                DrawTexture(heightMapTexture);
                break;

            case DrawMode.ColorMap:
                MeshRenderer.gameObject.SetActive(false);
                TextureRenderer.gameObject.SetActive(true);
                DrawTexture(colorMapTexture);
                break;

            case DrawMode.MeshNoiseTexture:
                MeshRenderer.gameObject.SetActive(true);
                TextureRenderer.gameObject.SetActive(false);
                DrawMesh(meshData, heightMapTexture);
                break;

            case DrawMode.MeshColorTexture:
                MeshRenderer.gameObject.SetActive(true);
                TextureRenderer.gameObject.SetActive(false);
                DrawMesh(meshData, colorMapTexture);
                break;
        }
    }

    public void HideMap()
    {
        MeshRenderer.gameObject.SetActive(false);
        TextureRenderer.gameObject.SetActive(false);
    }

    public void DrawTexture(Texture2D texture)
    {
        TextureRenderer.sharedMaterial.mainTexture = texture;
        TextureRenderer.transform.localScale = new Vector3(-(texture.width / 10f), 1, texture.height / 10f);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        MeshFilter.sharedMesh = meshData.CreateMesh();
        MeshRenderer.sharedMaterial.mainTexture = texture;
    }
}
