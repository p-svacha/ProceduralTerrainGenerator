using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] Vertices; // Array of vertex positions
    public Vector2[] UVs; // Relative position of each vertex on the map between 0 and 1
    public int[] Triangles; // Array of vertex Ids (position in vertices array) that the mesh triangles are formed from, 3 vertices -> 1 triange

    public int VertexIndex;
    public int UVIndex;
    public int TriangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        VertexIndex = 0;
        TriangleIndex = 0;
    }

    /// <summary>
    /// x, y, z are the world position of the vertex
    /// </summary>
    public int AddVertex(float x, float y, float z)
    {
        Vertices[VertexIndex] = new Vector3(x, y, z);
        VertexIndex++;
        return VertexIndex - 1;
    }

    /// <summary>
    /// x and y are the relative position of the vertex with the same index on the map (0-1)
    /// </summary>
    public void AddUV(float x, float y)
    {
        UVs[UVIndex] = new Vector2(x, y);
        UVIndex++;
    }

    /// <summary>
    /// a, b, c refer to vertex ids as they are saved in the array
    /// </summary>
    public void AddTriange(int a, int b, int c)
    {
        Triangles[TriangleIndex] = a;
        Triangles[TriangleIndex + 1] = b;
        Triangles[TriangleIndex + 2] = c;
        TriangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
