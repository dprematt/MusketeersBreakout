
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapdata;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailsLevel;
        LODMesh[] lodMeshes;

        bool mapDataReceived;

        int previousLOD;
        int previousLODIndex = -1;

        int waterThickness = 30;
        int sandThickness = 20;

        private generator _generator;

        public Vector2 ChunkCenter
        {
            get
            {
                return new Vector2(position.x + 240 / 2, position.y + 241 / 2);
            }
        }

        public Chunk(Vector2 coord, int size, LODInfo[] detailsLevel, Transform parent, Material material, Material grassMaterial, generator generatorInstance)
        {
            this._generator = generatorInstance;
            this.detailsLevel = detailsLevel;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Chunk");
            meshObject.layer = 6;
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshRenderer.materials = new Material[] { material, grassMaterial };
            meshObject.transform.position = positionV3 * 1;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * 1;
            SetVisible(true);

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
            this.mapdata = mapData;
            mapDataReceived = true;
            UpdateMesh();
            Vector2 chunkCenter = this.ChunkCenter;
            int seed = CalculateSeedForChunk(chunkCenter);
            _generator.PlacePrefabsInChunk(chunkCenter, mapData.heightMap, 240, new System.Random(seed));
            ApplyBeachesIfNeeded();
            UpdateChunk();
        }

        int CalculateSeedForChunk(Vector2 chunkCenter)
        {
            return chunkCenter.GetHashCode(); // Ou une autre méthode déterministe pour obtenir une graine constante basée sur la position du chunk
        }

    private void ApplyBeachesIfNeeded()
    {
        float mapHalfWidth = generator.mapChunkSize * 2.5f;
        float mapHalfHeight = generator.mapChunkSize * 2.5f;

        float chunkTopBorder = bounds.center.y + (bounds.size.y / 2);
        float chunkBottomBorder = bounds.center.y - (bounds.size.y / 2);
        float chunkRightBorder = bounds.center.x + (bounds.size.x / 2);
        float chunkLeftBorder = bounds.center.x - (bounds.size.x / 2);

        bool isTopBorderChunk = chunkTopBorder >= (mapHalfHeight - waterThickness) && bounds.center.y < mapHalfHeight;
        bool isBottomBorderChunk = chunkBottomBorder <= (-mapHalfHeight + waterThickness) && bounds.center.y > -mapHalfHeight;
        bool isLeftBorderChunk = chunkLeftBorder <= (-mapHalfWidth + waterThickness) && bounds.center.x > -mapHalfWidth;
        bool isRightBorderChunk = chunkRightBorder >= (mapHalfWidth - waterThickness) && bounds.center.x < mapHalfWidth;

        if (isTopBorderChunk)
            ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, true, false);
        if (isBottomBorderChunk)
            ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, false, false);
        if (isLeftBorderChunk)
            ApplyBeachAndSandToLeftOrRightBorder(waterThickness, sandThickness, true);
        if (isRightBorderChunk)
            ApplyBeachAndSandToLeftOrRightBorder(waterThickness, sandThickness, false);

        isTopBorderChunk = chunkTopBorder >= (mapHalfHeight - sandThickness - waterThickness) && bounds.center.y < mapHalfHeight;
        isBottomBorderChunk = chunkBottomBorder <= (-mapHalfHeight + sandThickness + waterThickness) && bounds.center.y > -mapHalfHeight;
        isLeftBorderChunk = chunkLeftBorder <= (-mapHalfWidth + sandThickness + waterThickness) && bounds.center.x > -mapHalfWidth;
        isRightBorderChunk = chunkRightBorder >= (mapHalfWidth - sandThickness - waterThickness) && bounds.center.x < mapHalfWidth;

        if (isTopBorderChunk)
            ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, true, true);
        if (isBottomBorderChunk)
            ApplyBeachAndSandToTopOrBottom(waterThickness, sandThickness, false, true);
    }

        private void ApplyBeachAndSandToTopOrBottom(int waterThickness, int sandThickness, bool isTopBorder, bool isWater)
        {
            float chunkBorderStartZ = isTopBorder ? bounds.center.y - (bounds.size.y / 2) : bounds.center.y + (bounds.size.y / 2);
            float sandEndZ = chunkBorderStartZ + (isTopBorder ? waterThickness : -waterThickness);
            float beachEndZ = sandEndZ + (isTopBorder ? sandThickness : -sandThickness);
            float mapHalfHeight = generator.mapChunkSize * 2.5f;

            System.Random prng = _generator.getPRNG(); // Get a random number generator

            for (int x = 0; x < generator.mapChunkSize; x++)
            {
                for (int z = 0; z < generator.mapChunkSize; z++)
                {
                    float worldZ = bounds.center.y - (bounds.size.y / 2) + z;

                    if ((isTopBorder && worldZ > mapHalfHeight - 30) || (!isTopBorder && worldZ < -mapHalfHeight + 30))
                    {
                        continue;
                    }

                    if (isWater)
                    {
                        float randomness = (float)prng.NextDouble() * 10f - 5f; // Adjust the range of randomness as needed
                        if (isTopBorder ? (worldZ >= chunkBorderStartZ && worldZ < sandEndZ + randomness) :
                                        (worldZ <= chunkBorderStartZ && worldZ > sandEndZ + randomness))
                        {
                            mapdata.heightMap[x, z] = 0.1f;
                        }
                    }
                    else
                    {
                        float randomness = (float)prng.NextDouble() * 10f - 5f; // Adjust the range of randomness as needed
                        if (isTopBorder ? (worldZ >= sandEndZ + randomness && worldZ < beachEndZ + randomness) :
                                        (worldZ <= sandEndZ + randomness && worldZ > beachEndZ + randomness))
                        {
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

            System.Random prng = _generator.getPRNG(); // Get a random number generator

            for (int x = 0; x < generator.mapChunkSize; x++)
            {
                for (int z = 0; z < generator.mapChunkSize; z++)
                {
                    float worldX = bounds.center.x - (bounds.size.x / 2) + x;

                    float randomness = (float)prng.NextDouble() * 10f - 5f; // Adjust the range of randomness as needed
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
            _generator.RequestMeshData(mapdata, previousLOD, (meshData) => {
                meshFilter.mesh = meshData.CreateMesh();
                meshCollider.sharedMesh = meshData.CreateMesh();
            });
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

                            if (lodIndex >= 1)
                                meshRenderer.materials[1].EnableKeyword("_GRASS_ON");
                            else
                                meshRenderer.materials[1].DisableKeyword("_GRASS_ON");
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapdata);
                        }
                    }
                }
                SetVisible(true);
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
    }