using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{

    public enum DrawMode {map, colorMap, mesh}
    public DrawMode drawMode;
    const int mapChunckSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offSet;

    public bool useFallOf;

    
    public float meshHeightMult;
    public AnimationCurve meshHeightCurve;
    float[,] fallOfMap;

    public TerrainType[] regions;

    //private void awake() {
      //  fallOfMap = FallOfMapGenerator.GenerateFallOfMap(mapChunckSize);
    //}
    public void SkeletonGenerator() {
        float[,] map = Skeleton.GenerateSkeleton(mapChunckSize, mapChunckSize, scale, octaves, persistance, lacunarity, offSet); 

        Color[] colorMap = new Color[mapChunckSize * mapChunckSize];

        for (int i = 0; i < mapChunckSize; i++) {

            for (int j = 0; j < mapChunckSize; j++) {

                float curHeight = map[j,i];

                for (int k = 0; k < regions.Length; k++) {

                    if (curHeight <= regions[k].height) {

                        colorMap[i * mapChunckSize + j]  = regions[k].colour;
                        break;
                    }

                }
            }
        }   

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map) {

            display.DrawTexture(TextureGenerator.TextureFromHeightMap(map));

        } else if (drawMode == DrawMode.colorMap) {

            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunckSize, mapChunckSize));
        } else if (drawMode == DrawMode.mesh) {
            display.DrawMesh(MeshGenerator.generateTerrainMesh(map, meshHeightMult, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunckSize, mapChunckSize));
        }
    }
     private void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}