using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endless : MonoBehaviour
{
    const float scale = 1;
    public static float maxViewDist;

    public LODInfo[] detailsLevel;
    public static Vector2 viewerPosition;
    Vector2 oldViewerPosition;

    public Material mapMaterial;
    public Material grassMaterial;
    static generator _generator;
    int chunkSize;
    int chunkVisibleViewDist;
    Dictionary<Vector2, Chunk> chunkDict = new Dictionary<Vector2, Chunk>();

    Vector2 placementAreaSize = new Vector2(1500, 1500);
    static List<Chunk> chunkVisibleLastUpdate = new List<Chunk>();

    private void Start() {
        _generator = FindObjectOfType<generator>();
        StartCoroutine(SetupPrefabsAndTerrain());
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = generator.mapChunkSize - 1;
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
                    chunkDict[viewedChunkCoord] = new Chunk(viewedChunkCoord, chunkSize, detailsLevel, transform, mapMaterial, grassMaterial);
                }
            }
        }
    }


    private void Update()
    {
    }

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

        public Vector2 ChunkCenter
        {
            get
            {
                return new Vector2(position.x + 240 / 2, position.y + 241 / 2);
            }
        }

        public Chunk(Vector2 coord, int size, LODInfo[] detailsLevel, Transform parent, Material material, Material grassMaterial)
        {
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
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(true);

            lodMeshes = new LODMesh[detailsLevel.Length];

            for (int i = 0; i < detailsLevel.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailsLevel[i].lod, UpdateChunk);
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
            _generator.PlacePrefabsInChunk(chunkCenter, mapData.heightMap, 240, _generator.getPRNG());
            ApplyBeachesIfNeeded();
            UpdateChunk();
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
                        if (isTopBorder ? (worldZ >= chunkBorderStartZ && worldZ < sandEndZ) :
                                        (worldZ <= chunkBorderStartZ && worldZ > sandEndZ))
                        {
                            mapdata.heightMap[x, z] = 0.1f;
                        }
                    }
                    else
                    {
                        if (isTopBorder ? (worldZ >= sandEndZ && worldZ < beachEndZ) :
                                        (worldZ <= sandEndZ && worldZ > beachEndZ))
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

            for (int x = 0; x < generator.mapChunkSize; x++)
            {
                for (int z = 0; z < generator.mapChunkSize; z++)
                {
                    float worldX = bounds.center.x - (bounds.size.x / 2) + x;

                    if (isLeftBorder ? (worldX >= chunkBorderStartX && worldX < sandEndX) :
                                    (worldX <= chunkBorderStartX && worldX > sandEndX))
                    {
                        mapdata.heightMap[x, z] = 0.1f;
                    }
                    else if (isLeftBorder ? (worldX >= sandEndX && worldX < beachEndX) :
                                        (worldX <= sandEndX && worldX > beachEndX))
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
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
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
                    chunkVisibleLastUpdate.Add(this);
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

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallBack;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallBack = updateCallback;
        }

        void OnMeshDataReceived(meshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;
            updateCallBack();
        }

        public void RequestMesh(MapData mapdata)
        {
            hasRequestedMesh = true;
            _generator.RequestMeshData(mapdata, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
    }
}
