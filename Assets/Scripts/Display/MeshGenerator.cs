using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(MapData mapData, float heightMultiplier, int levelOfDetail)
    {
        int width = mapData.HeightMap.GetLength(0);
        int height = mapData.HeightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData MeshData = new MeshData(verticesPerLine, verticesPerLine);

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for(int x = 0; x < width; x += meshSimplificationIncrement)
            {
                // Add Vertex for this position (index of vertex is width*yPos + xPos)
                int vertexIndex = MeshData.AddVertex(topLeftX + x, mapData.HeightMap[x, y] * heightMultiplier, topLeftZ - y);

                // Add UV (relative position of vertex on the map)
                MeshData.AddUV(x / (float)width, y / (float)height);

                // Add the 2 triangles facing down right if we're not at the right or bottom border (because there are no more triangles facing bottom right)
                // Triangles are constructed clockwise!
                if (x < width - 1 && y < height - 1)
                {
                    // Create Triangle that goes from current vertex (i) > vertex below right from current vertext (i+width+1) > vertex below current vertex (i+width)
                    MeshData.AddTriange(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);

                    // Create Triangle that goes from vertex below right current vertex (i+width+1) > current vertex (i) > vertex right from current vertext (i+1)
                    MeshData.AddTriange(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
            }
        }

        return MeshData;
    }
}
