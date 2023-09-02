using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{

    public enum DrawMode {map, colorMap}
    public DrawMode drawMode;

    public int width;
    public int height;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;

    public Vector2 offSet;

    public TerrainType[] regions;
    public void SkeletonGenerator() {
        float[,] map = Skeleton.GenerateSkeleton(width, height, scale, octaves, persistance, lacunarity, offSet); 

        Color[] colorMap = new Color[width * height];

        for (int i = 0; i < height; i++) {

            for (int j = 0; j < width; j++) {

                float curHeight = map[j,i];

                for (int k = 0; k < regions.Length; k++) {

                    if (curHeight <= regions[k].height) {

                        colorMap[i * width + j]  = regions[k].colour;
                        break;
                    }

                }
            }
        }   

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map) {

            display.DrawTexture(TextureGenerator.TextureFromHeightMap(map));

        } else if (drawMode == DrawMode.colorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, width, height));
        }
    }
     private void OnValidate()
    {
        if(width < 1)
        {
            width = 1;
        }
        if(height < 1)
        {
            height = 1;
        }
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