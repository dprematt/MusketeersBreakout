using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class Skeleton
{
    public enum NormalizeMode { Local, Global }
    public static float[,] GenerateSkeleton(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] skeleton = new float[mapWidth, mapHeight];

        float maxPossibleHeight = 0;
        System.Random random = new System.Random();
        int seed = random.Next(1,1);
        System.Random prng = new System.Random(seed);
        float amplitude = 1;
        float frequency = 1;

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency ;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                skeleton[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local) {

                    skeleton[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, skeleton[x, y]);
                }
                else {
                    float normalizeHeight = (skeleton[x,y] + 1) / (maxPossibleHeight);
                    skeleton[x,y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                }
            }
        }

        return skeleton;
    }
}