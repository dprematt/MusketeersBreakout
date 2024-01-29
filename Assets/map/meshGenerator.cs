using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static meshData generateTerrainMesh(float[,] heightMap, float heightMultiplieur, AnimationCurve meshHeightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(meshHeightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplification = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplification + 1;


        meshData meshData = new meshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int i = 0; i < height; i += meshSimplification)
        {
            for (int j = 0; j < width; j += meshSimplification)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + j, heightCurve.Evaluate(heightMap[j, i]) * heightMultiplieur, topLeftZ - i);
                meshData.UVs[vertexIndex] = new Vector2(j / (float)width, i / (float)height);

                if (j < width - 1 && i < height - 1)
                {
                    meshData.addTriangles(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.addTriangles(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;

            }
        }
        return meshData;
    }

}

public class meshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] UVs;
    int trianglesIndex;
    public meshData(int meshWidth, int meshHeight)
    {

        vertices = new Vector3[meshHeight * meshWidth];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        UVs = new Vector2[meshWidth * meshHeight];
    }

    public void addTriangles(int a, int b, int c)
    {
        triangles[trianglesIndex] = a;
        triangles[trianglesIndex + 1] = b;
        triangles[trianglesIndex + 2] = c;
        trianglesIndex += 3;
    }
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();

        return mesh;
    }
}