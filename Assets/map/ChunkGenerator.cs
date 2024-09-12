using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    const float scale = 1;
    public static float maxViewDist;

    public LODInfo[] detailsLevel;
    public Material mapMaterial;
    public Material grassMaterial;
    static Generator _generator;
    int chunkSize;
    int chunkVisibleViewDist;
    Dictionary<Vector2, Chunk> chunkDict = new Dictionary<Vector2, Chunk>();

    public const int mapChunkSize = 241;


    Vector2 placementAreaSize = new Vector2(1500, 1500);
    private void Start() {
        _generator = FindObjectOfType<Generator>();
        StartCoroutine(SetupPrefabsAndTerrain());
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = mapChunkSize - 1;
        chunkVisibleViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        placementAreaSize = new Vector2(chunkSize * 5, chunkSize * 5);
        GenerateAllChunks();
    }

    IEnumerator SetupPrefabsAndTerrain()
    {
        yield return new WaitUntil(() => _generator.CurrentMapData.heightMap != null);
    }

    private void GenerateAllChunks()
    {
        int startChunkX = -(int)(placementAreaSize.x / 2 / chunkSize);
        int endChunkX = (int)(placementAreaSize.x / 2 / chunkSize);
        int startChunkY = -(int)(placementAreaSize.y / 2 / chunkSize);
        int endChunkY = (int)(placementAreaSize.y / 2 / chunkSize);

        for (int yOffset = startChunkY; yOffset <= endChunkY; yOffset++)
        {
            for (int xOffset = startChunkX; xOffset <= endChunkX; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                if (!chunkDict.ContainsKey(viewedChunkCoord))
                {
                    chunkDict[viewedChunkCoord] = new Chunk(viewedChunkCoord, chunkSize, detailsLevel, transform, mapMaterial, grassMaterial, _generator);
                }
            }
        }
    }


    private void Update()
    {
    }
}
