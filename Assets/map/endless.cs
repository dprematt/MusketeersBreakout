using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Endless : MonoBehaviour {
    const float scale = 1;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static float maxViewDist;

    public LODInfo[] detailsLevel;
    public Transform viewer;
    public static Vector2 viewerPosition;
    Vector2 oldViewerPosition;

    public  Material mapMaterial;
    static generator _generator; 
    int chunkSize;
    int chunkVisibleViewDist;
    Dictionary<Vector2, Chunk> chunkDict = new Dictionary<Vector2, Chunk>();
    static List<Chunk> chunkVisibleLastUpdate = new List<Chunk>();
    private void Start() {
        _generator = FindObjectOfType<generator>();
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = generator.mapChunckSize - 1;
        chunkVisibleViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);

        UpdateVisibleChunk(); 
        
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / 2f;

    
        viewerPosition = new Vector2(
            Mathf.Clamp(viewerPosition.x, -550, 550),
            Mathf.Clamp(viewerPosition.y, -550, 550)
        );

        if ((oldViewerPosition - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
            oldViewerPosition = viewerPosition;
            UpdateVisibleChunk();
        }
    }

    void UpdateVisibleChunk() {    
        int centralChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int centralChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        int mapRadius = 750;
        int startChunkX = centralChunkCoordX - Mathf.CeilToInt((float)mapRadius / chunkSize);
        int endChunkX = centralChunkCoordX + Mathf.CeilToInt((float)mapRadius / chunkSize);
        int startChunkY = centralChunkCoordY - Mathf.CeilToInt((float)mapRadius / chunkSize);
        int endChunkY = centralChunkCoordY + Mathf.CeilToInt((float)mapRadius / chunkSize);

        for (int i = 0; i < chunkVisibleLastUpdate.Count; i++) {
            chunkVisibleLastUpdate[i].SetVisible(false);
        }
        chunkVisibleLastUpdate.Clear();

        for (int yOffset = startChunkY; yOffset <= endChunkY; yOffset++) {
            for (int xOffset = startChunkX; xOffset <= endChunkX; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);


                if (chunkDict.ContainsKey(viewedChunkCoord)) {
                    chunkDict[viewedChunkCoord].UpdateChunk();
                    if (chunkDict[viewedChunkCoord].isVisible()) {
                        chunkVisibleLastUpdate.Add(chunkDict[viewedChunkCoord]);
                    }
                } else {
                    chunkDict.Add(viewedChunkCoord, new Chunk(viewedChunkCoord, chunkSize, detailsLevel, transform, mapMaterial));
                }
            }
        }
    }
    public class Chunk {
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
        public Chunk(Vector2 coord, int size, LODInfo[] detailsLevel, Transform parent, Material material) {
            this.detailsLevel = detailsLevel;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            
            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshRenderer.material = material;
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale; 
            SetVisible(false);

            lodMeshes = new LODMesh[detailsLevel.Length];

            for (int i = 0; i < detailsLevel.Length; i++) {
                lodMeshes[i] = new LODMesh(detailsLevel[i].lod, UpdateChunk);
            }
            _generator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData) {
            this.mapdata = mapData;
            mapDataReceived = true;
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, generator.mapChunckSize, generator.mapChunckSize);
            meshRenderer.material.mainTexture = texture;
            UpdateChunk();
        }

        public void UpdateChunk() {
            if (mapDataReceived) {
                float viewerDstFromNearestEdge =  Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDist;

                if (visible) {
                    int lodIndex = 0;
                    for (int i = 0; i < detailsLevel.Length - 1; i ++) {
                        if (viewerDstFromNearestEdge > detailsLevel[i].visibleDstThreshold) {
                            lodIndex = i + 1;
                        } else {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex) {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh) 
                        {   
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh;
                        } 
                        else if (!lodMesh.hasRequestedMesh) {
                            lodMesh.RequestMesh(mapdata);
                        }
                    }
                    chunkVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }


        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool  isVisible() {
            return meshObject.activeSelf;
        }
    }

    class LODMesh {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallBack;

        public LODMesh(int lod, System.Action updateCallback) {
            this.lod = lod;
            this.updateCallBack = updateCallback;
        }
        void OnMeshDataReceived(meshData meshdata) {
            mesh = meshdata.CreateMesh();
            hasMesh = true;

            updateCallBack();
        }
        public void RequestMesh(MapData mapdata) {
            hasRequestedMesh = true;
            _generator.RequestMeshData(mapdata, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float visibleDstThreshold;
    }
}