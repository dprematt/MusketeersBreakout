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

    private void Start()
    {
        _generator = FindObjectOfType<generator>();
        StartCoroutine(SetupPrefabsAndTerrain());
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = generator.mapChunkSize - 1;
        chunkVisibleViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
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
        // Plus besoin de code lié à la position du viewer.
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
            UpdateChunk();
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
