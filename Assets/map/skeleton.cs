using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class Skeleton
{
    public enum NormalizeMode { Local, Global }
    public static (float[,], List<Vector2>) GenerateSkeleton(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode, int seed) {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        float[,] skeleton = new float[mapWidth, mapHeight];

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                skeleton[x, y] = noiseHeight;
            }
        }

        normalizeMode = NormalizeMode.Global;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if (normalizeMode == NormalizeMode.Local) {
                    skeleton[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, skeleton[y, x]);
                } else {
                    float normalizeHeight = (skeleton[x, y] + 1) / (2 * maxPossibleHeight / 1.5f);
                    skeleton[x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                }
            }
        }

        float flatHeight = 0.4f;
        int numberOfPlates = 6;
        List<Vector2> plateCenters = new List<Vector2>();

        System.Random localPrng = new System.Random(seed);
        while (plateCenters.Count < numberOfPlates) {
            Vector2 newCenter = new Vector2(localPrng.Next(20, mapWidth - 20) + offset.x, localPrng.Next(20, mapHeight - 20) + offset.y);
            if (!plateCenters.Any(center => Vector2.Distance(center, newCenter) < 70)) {
                plateCenters.Add(newCenter);
            }
        }

        foreach (var center in plateCenters) {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    float distance = Vector2.Distance(new Vector2(x + offset.x, y + offset.y), center);
                    float perlinNoise = Mathf.PerlinNoise((x + offset.x) * 0.1f, (y + offset.y) * 0.1f) * 20f;

                    if (distance <= 60 + perlinNoise) {
                        skeleton[x, y] = flatHeight;
                    } 
                    else if (distance > 60 + perlinNoise && distance <= 90 + perlinNoise) {
                        float transition = Mathf.InverseLerp(90 + perlinNoise, 60 + perlinNoise, distance);

                        float blendedHeight = Mathf.Lerp(flatHeight, skeleton[x, y], transition);
                        
                        float averageHeight = 0;
                        int count = 0;

                        for (int offsetY = -1; offsetY <= 1; offsetY++) {
                            for (int offsetX = -1; offsetX <= 1; offsetX++) {
                                int neighborX = Mathf.Clamp(x + offsetX, 0, mapWidth - 1);
                                int neighborY = Mathf.Clamp(y + offsetY, 0, mapHeight - 1);
                                averageHeight += skeleton[neighborX, neighborY];
                                count++;
                            }
                        }

                        averageHeight /= count;

                        skeleton[x, y] = Mathf.Lerp(blendedHeight, averageHeight, 0.5f);
                    }
                }
            }
        }

        Debug.Log($"Total des zones plates -> {plateCenters}");
        return (skeleton, plateCenters);
    }
}