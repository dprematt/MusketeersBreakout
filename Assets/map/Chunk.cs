using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Environment.Instancing;

public class Chunk
{
    public GameObject meshObject;
    Vector2 position;
    Bounds bounds;

    MapData mapdata;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailsLevel;
    LODMesh[] lodMeshes;

    bool mapDataReceived;

    int previousLODIndex = -1;

    int waterThickness = 30;
    int sandThickness = 20;

    private Generator _generator;
    public const int mapChunkSize = 241;

    private Mesh[] _instanceMeshes;
    private Material[] _instanceMaterials;

    public Vector2 ChunkCenter
    {
        get
        {
            return new Vector2(position.x + 240 / 2, position.y + 241 / 2);
        }
    }

    public Chunk(Vector2 coord, int size, LODInfo[] detailsLevel, Transform parent, Material material, Generator generatorInstance, Mesh[] instanceMeshes, Material[] instanceMaterials)
    {
        this._generator = generatorInstance;
        this.detailsLevel = detailsLevel;

        _instanceMaterials = instanceMaterials;
        _instanceMeshes = instanceMeshes;

        position = coord * size;
        bounds = new Bounds(position, Vector2.one * size);
        Vector3 positionV3 = new Vector3(position.x, 0, position.y);

        meshObject = new GameObject("Chunk");
        meshObject.layer = 6;
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshRenderer.materials = new Material[] { material };
        meshObject.transform.position = positionV3 * 1;
        meshObject.transform.parent = parent;
        meshObject.transform.localScale = Vector3.one * 1;
        SetVisible(false);

        lodMeshes = new LODMesh[detailsLevel.Length];

        for (int i = 0; i < detailsLevel.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailsLevel[i].lod, UpdateChunk, _generator);
        }
        _generator.RequestMapData(position, OnMapDataReceived);
}
    
    bool IsPositionInChunk(Vector3 localPosition)
    {
        return bounds.Contains(new Vector3(localPosition.x, 0, localPosition.z));
    }

    void OnMapDataReceived(MapData mapData)
    {
        try {
            this.mapdata = mapData;
            mapDataReceived = true;
            UpdateMesh();
            Vector2 chunkCenter = this.ChunkCenter;
            int seed = CalculateSeedForChunk(chunkCenter);
            _generator.PlacePrefabsInChunk(chunkCenter, mapData.heightMap, 240, new System.Random(seed));
            ApplyBeachesIfNeeded();
            UpdateChunk();
    } catch (Exception ex) {
            Debug.LogError($"Erreur dans l'update du chunk: {ex.Message}");
        }
    }

    int CalculateSeedForChunk(Vector2 chunkCenter)
    {
        return chunkCenter.GetHashCode();
    }

    private void ApplyBeachesIfNeeded()
    {
        float mapHalfWidth = mapChunkSize * 2.0f;
        float mapHalfHeight = mapChunkSize * 2.0f;

        float chunkTopBorder = bounds.center.y + (bounds.size.y / 2);
        float chunkBottomBorder = bounds.center.y - (bounds.size.y / 2);
        float chunkRightBorder = bounds.center.x + (bounds.size.x / 2);
        float chunkLeftBorder = bounds.center.x - (bounds.size.x / 2);

        bool isTopBorderChunk = chunkTopBorder >= (mapHalfHeight - waterThickness - mapChunkSize ) && bounds.center.y < mapHalfHeight;
        bool isRightBorderChunk = chunkRightBorder >= (mapHalfWidth - waterThickness - mapChunkSize) && bounds.center.x < mapHalfWidth;
        bool isBottomBorderChunk = chunkBottomBorder <= (-mapHalfHeight + waterThickness) && bounds.center.y > -mapHalfHeight;
        bool isLeftBorderChunk = chunkLeftBorder <= (-mapHalfWidth + waterThickness) && bounds.center.x > -mapHalfWidth;

        try {
            if (isTopBorderChunk)
                ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, true, false); 
            if (isBottomBorderChunk)
                ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, false, false);
            if (isLeftBorderChunk)
                ApplyBeachAndSandToLeftOrRightBorder(waterThickness, sandThickness, true);
            if (isRightBorderChunk)
                ApplyBeachAndSandToLeftOrRightBorder(waterThickness, sandThickness, false);
            if (isTopBorderChunk)
                ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, true, true); 
            if (isBottomBorderChunk)
                ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, false, true);
        } catch (Exception ex) {
            Debug.LogError($"Erreur lors de la génération de la plage sur les chunks: {ex.Message}");
        }
    }

    private void ApplyBeachAndSandToTopOrBottom(int waterThickness, int sandThickness, bool isTopBorder, bool isWater)
    {
        float chunkBorderStartZ = isTopBorder ? bounds.center.y - (bounds.size.y / 2) : bounds.center.y + (bounds.size.y / 2);
        float sandEndZ = chunkBorderStartZ + (isTopBorder ? waterThickness : -waterThickness);
        float beachEndZ = sandEndZ + (isTopBorder ? sandThickness : -sandThickness);
        float mapHalfHeight = mapChunkSize * 2.0f;

        System.Random prng = _generator.getPRNG();

        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int z = 0; z < mapChunkSize; z++)
            {
                float worldZ = bounds.center.y - (bounds.size.y / 2) + z;

                if ((isTopBorder && worldZ > mapHalfHeight - 30) || (!isTopBorder && worldZ < -mapHalfHeight + 30))
                {
                    continue;
                }

                float randomness = (float)prng.NextDouble() * 10f - 5f;
                if (isWater) {
                    if (isTopBorder ? (worldZ >= chunkBorderStartZ && worldZ < sandEndZ + randomness) :
                                    (worldZ <= chunkBorderStartZ && worldZ > sandEndZ + randomness))
                    {
                        mapdata.heightMap[x, z] = 0.1f;
                    }
                }
                else {
                    if (isTopBorder ? (worldZ >= sandEndZ + randomness && worldZ < beachEndZ + randomness) :
                        (worldZ <= sandEndZ + randomness && worldZ > beachEndZ + randomness)) {
                            mapdata.heightMap[x, z] = 0.15f;
                    }
                }
            }
        }
    }

    private void ApplyBeachAndSandToLeftOrRightBorder(int waterThickness, int sandThickness, bool isLeftBorder)
    {
        float chunkBorderStartX = isLeftBorder ? bounds.center.x - (bounds.size.x / 2) : bounds.center.x + (bounds.size.x / 2);
        float sandEndX = chunkBorderStartX + (isLeftBorder ? waterThickness : -waterThickness);
        float beachEndX = sandEndX + (isLeftBorder ? sandThickness : -sandThickness);

        System.Random prng = _generator.getPRNG();

        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int z = 0; z < mapChunkSize; z++)
            {
                float worldX = bounds.center.x - (bounds.size.x / 2) + x;

                float randomness = (float)prng.NextDouble() * 10f - 5f;
                if (isLeftBorder ? (worldX >= chunkBorderStartX && worldX < sandEndX + randomness) :
                                (worldX <= chunkBorderStartX && worldX > sandEndX + randomness))
                {
                    mapdata.heightMap[x, z] = 0.1f;
                }
                else if (isLeftBorder ? (worldX >= sandEndX + randomness && worldX < beachEndX + randomness) :
                                    (worldX <= sandEndX + randomness && worldX > beachEndX + randomness))
                {
                    mapdata.heightMap[x, z] = 0.15f;
                }
            }
        }
    }

    private void UpdateMesh()
    {
        SetVisible(true);
    }

    public void UpdateChunk()
    {
        if (mapDataReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(new Vector2(0,0)));
            bool visible = true;

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailsLevel.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailsLevel[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                        meshCollider.sharedMesh = lodMesh.mesh;

                        /*if (lodIndex >= 1)
                            meshRenderer.materials[1].EnableKeyword("_GRASS_ON");
                        else
                            meshRenderer.materials[1].DisableKeyword("_GRASS_ON");*/
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapdata);
                    }
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool isVisible()
    {
        return meshObject.activeSelf;
    }

    public void InstantiateGrass()
    {
        // Ajouter le composant MeshInstancesBehaviour
        MeshInstancesBehaviour meshInstancesBehaviour = meshObject.AddComponent<MeshInstancesBehaviour>();

        // Configurer les propriétés de MeshInstancesBehaviour
        meshInstancesBehaviour.Density = 1f;
        meshInstancesBehaviour.InstanceConfigurations = GenerateInstanceConfigurations(_instanceMeshes, _instanceMaterials);
    }

    private InstanceConfiguration[] GenerateInstanceConfigurations(Mesh[] instanceMeshes, Material[] instanceMaterials)
    {
        InstanceConfiguration[] configurations = new InstanceConfiguration[instanceMeshes.Length];
        for (int i = 0; i < instanceMeshes.Length; i++)
        {
            configurations[i] = new InstanceConfiguration
            {
                Mesh = instanceMeshes[i],
                Material = instanceMaterials[i],
                Probability = i == 0 ? 100 : 1,
                Scale = 1.0f,
                NormalOffset = i == 0 ? 0.1f : 0.5f
            };
        }
        return configurations;
    }
}