using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BiomesGenerator
{
    public static Texture2D GenerateBiomes(Color[] colorMap, int width, int height)
    {
        System.Random random = new System.Random();

        List<Vector2Int> squarePositions = new List<Vector2Int>();
        int squareSize = 16;

        Color[] biomeColors = new Color[]
        {
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.green,
            Color.black,
            Color.magenta,
            Color.red
        };

        Dictionary<Color, List<Vector2Int>> placedSquares = new Dictionary<Color, List<Vector2Int>>();
        foreach (Color color in biomeColors)
        {
            placedSquares[color] = new List<Vector2Int>();
        }

        for (int i = 0; i < biomeColors.Length; i++)
        {
            Color currentColor = biomeColors[i];

            Vector2Int squarePosition = GetValidSquarePosition(width, height, squareSize, squarePositions, random, currentColor, placedSquares);
            squarePositions.Add(squarePosition);
            placedSquares[currentColor].Add(squarePosition);

            for (int y = squarePosition.y; y < squarePosition.y + squareSize; y++)
            {
                for (int x = squarePosition.x; x < squarePosition.x + squareSize; x++)
                {
                    float perlinValue = Mathf.PerlinNoise(x, y);
                    colorMap[y * width + x] = currentColor;
                }
            }
        }

        return TextureFromBiomes(colorMap, width, height);
    }

   private static Vector2Int GetValidSquarePosition(int width, int height, int squareSize, List<Vector2Int> existingPositions, System.Random random, Color currentColor, Dictionary<Color, List<Vector2Int>> placedSquares)
        {
        int maxAttempts = 100; 
        int currentAttempt = 0;

        System.Random firstSeed = new System.Random();
        int seed = firstSeed.Next(1,50);

        while (currentAttempt < maxAttempts)
        {
            int x = random.Next(seed, width - squareSize);
            int y = random.Next(seed, height - squareSize);

            Vector2Int squarePosition = new Vector2Int(x, y);

            bool overlaps = false;
            foreach (Vector2Int existingPosition in existingPositions)
            {
                if (IsOverlap(squarePosition, squareSize, existingPosition, squareSize))
                {
                    overlaps = true;
                    break;
                }
            }

            foreach (Vector2Int existingPosition in placedSquares[currentColor])
            {
                if (Vector2Int.Distance(squarePosition, existingPosition) < squareSize * 2)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
                return squarePosition;

            currentAttempt++;
        }
        return new Vector2Int(0, 0);
        }


    private static bool IsOverlap(Vector2Int position1, int size1, Vector2Int position2, int size2)
    {
        return position1.x < position2.x + size2 && position1.x + size1 > position2.x &&
               position1.y < position2.y + size2 && position1.y + size1 > position2.y;
    }

    public static Texture2D TextureFromBiomes(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}