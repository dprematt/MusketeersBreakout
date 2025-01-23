using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Environment.Instancing;

public class ChunkGenerator : MonoBehaviour
{
    const float scale = 1;
    public static float maxViewDist;

    public LODInfo[] detailsLevel;
    public Material mapMaterial;
    public Mesh[] instanceMeshes;
    public Material[] instanceMaterials;
    static Generator _generator;
    int chunkSize;
    int chunkVisibleViewDist;
    Dictionary<Vector2, Chunk> chunkDict = new Dictionary<Vector2, Chunk>();

    public const int mapChunkSize = 241;

    Vector2 placementAreaSize;

    private void Start() {
        _generator = FindObjectOfType<Generator>();
        StartCoroutine(SetupPrefabsAndTerrain());
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = mapChunkSize - 1;
        chunkVisibleViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        placementAreaSize = new Vector2(chunkSize * 4, chunkSize * 4);
        GenerateAllChunks();
    }

    IEnumerator SetupPrefabsAndTerrain()
    {
        yield return new WaitUntil(() => _generator.CurrentMapData.heightMap != null);
    }

    private void GenerateAllChunks()
    {
        try {
            int startChunkX = -(int)(placementAreaSize.x / 2 / chunkSize);
            int endChunkX = (int)(placementAreaSize.x / 2 / chunkSize) - 1;
            int startChunkY = -(int)(placementAreaSize.y / 2 / chunkSize);
            int endChunkY = (int)(placementAreaSize.y / 2 / chunkSize) - 1;

            for (int yOffset = startChunkY; yOffset <= endChunkY; yOffset++)
            {
                for (int xOffset = startChunkX; xOffset <= endChunkX; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                    if (!chunkDict.ContainsKey(viewedChunkCoord))
                    {
                        chunkDict[viewedChunkCoord] = new Chunk(viewedChunkCoord, chunkSize, detailsLevel, transform, mapMaterial, _generator, instanceMeshes, instanceMaterials);
                    }
                }
            }
        } catch (Exception ex) {
            Debug.LogError($"Erreur durant la génération de tous les chunks: {ex.Message}");
        }

    }

    private void Update()
    {
    }
}
