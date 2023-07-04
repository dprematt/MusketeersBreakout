using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Skeleton
{
    //
    // This function generates the map's skeleton
    // param width : map's width
    // param height : map's height
    // param scale : float which will determine the level of relief
    // return : float array representing the map
    //
    public static float[,] GenerateSkeleton(int width, int height, float scale)
    {
        float[,] skeleton = new float[width, height];
        
        if (scale <= 0) {
            scale = 0.001f; //default value
        }

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {

                float j_ = j / scale;
                float i_ = i / scale;

                float perlin = Mathf.PerlinNoise(j_, i_); // generate the pixel relief
                skeleton[j, i] = perlin;
            }
        }
        return skeleton;
    }
}