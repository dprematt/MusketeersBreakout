using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

    public static meshData generateTerrainMesh(float[,] heightMap, float heightMultiplieur, AnimationCurve meshHeightCurve, int levelOfDetail) {

        AnimationCurve heightCurve = new AnimationCurve(meshHeightCurve.keys);
        int meshSimplification = (levelOfDetail == 0) ?1:levelOfDetail * 2;
        int borderSize = heightMap.GetLength(0);
        int meshSize = borderSize - 2 * meshSimplification;
        int meshSizeUnsimplified = borderSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;
        int verticesPerLine = (meshSizeUnsimplified - 1) / meshSimplification + 1;


        meshData meshData = new meshData(verticesPerLine);
        int[,] vertexIndicesMap = new int[borderSize, borderSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;


        for (int y = 0; y < borderSize; y += meshSimplification) 
        {
            for (int x = 0; x < borderSize; x += meshSimplification) 
            {
                bool isBorderVertex = y == 0 || y == borderSize -1 || x == 0 || x == borderSize -1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x,y] = borderVertexIndex;
                    borderVertexIndex--;
                } 
                else 
                {
                    vertexIndicesMap[x,y] = meshVertexIndex;
                    meshVertexIndex++;
                    
                }
            }
        }

        for (int i = 0; i < borderSize; i += meshSimplification) {
            for (int j = 0; j < borderSize; j += meshSimplification) {

                int vertexIndex = vertexIndicesMap[j,i];

                Vector2 percent = new Vector2((j - meshSimplification) / (float)meshSize, (i - meshSimplification) / (float)meshSize);
                float heigth = heightCurve.Evaluate(heightMap[j, i]) * heightMultiplieur;
                Vector3 vertexPos = new Vector3(topLeftX + percent.x * meshSize, heigth, topLeftZ - percent.y * meshSize);

                meshData.addVertex(vertexPos, percent, vertexIndex);
                if (j < borderSize - 1 && i < borderSize - 1) 
                {
                    int a = vertexIndicesMap[j,i];
                    int b = vertexIndicesMap[j + meshSimplification, i];
                    int c = vertexIndicesMap[j,i + meshSimplification];
                    int d = vertexIndicesMap[j + meshSimplification,i + meshSimplification];

                    meshData.addTriangle(a,d,c);
                    meshData.addTriangle(d, a, b);
                }
                vertexIndex ++;

            }
        }
        return meshData;
    }

}

public class meshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    Vector3[] borderVertices;
    int[] borderTriangles;

    int triangleIndex;
    int borderTriangleIndex;

    public meshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        borderVertices = new Vector3[verticesPerLine * 4 + 4];
        borderTriangles = new int[24 * verticesPerLine];
    }

    public void addVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if(vertexIndex < 0)
        {
            borderVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void addTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    Vector3[] calculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if(vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if(vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if(vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
            
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0)? borderVertices[-indexA-1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = calculateNormals();
        return mesh;
    }
}