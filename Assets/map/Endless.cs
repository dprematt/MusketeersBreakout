using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Endless : MonoBehaviour {
    const float scale = 1;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static float maxViewDist;

    float mapSize = 1500f;
    float safeZone = 100f;

    int numberOfPrefabsToCreate = 2;

    public GameObject[] prefabTypes;

    public LODInfo[] detailsLevel;
    public Transform viewer;
    public static Vector2 viewerPosition;
    Vector2 oldViewerPosition;

    public  Material mapMaterial;
    static generator _generator; 
    int chunkSize;
    int chunkVisibleViewDist;
    Dictionary<Vector2, Chunk> chunkDict = new Dictionary<Vector2, Chunk>();

    Vector2 placementAreaSize = new Vector2(1500, 1500); 
    static List<Chunk> chunkVisibleLastUpdate = new List<Chunk>();

    public static List<Vector3> prefabPositions = new List<Vector3>();
    private void Start() {
        _generator = FindObjectOfType<generator>();   
        StartCoroutine(SetupPrefabsAndTerrain());
        maxViewDist = detailsLevel[detailsLevel.Length - 1].visibleDstThreshold;
        chunkSize = generator.mapChunckSize - 1;
        chunkVisibleViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        UpdateVisibleChunk(); 
        
    }
    IEnumerator SetupPrefabsAndTerrain() {
        yield return new WaitUntil(() => _generator.CurrentMapData.heightMap != null);
        PlaceRandomPrefabs(_generator.CurrentMapData.heightMap);
    }
    private void Update() {
        GameObject player = GameObject.Find("Player(Clone)");
        if (player != null) {
            viewer = player.transform;
            Debug.LogWarning("Player has been found");

        } else {
            Debug.LogWarning("Le GameObject 'Player(Clone)' n'a pas été trouvé.");
        }
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
    
        viewerPosition = new Vector2(
            Mathf.Clamp(viewerPosition.x, -550, 550),
            Mathf.Clamp(viewerPosition.y, -550, 550)
        );

        if ((oldViewerPosition - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
            oldViewerPosition = viewerPosition;
            UpdateVisibleChunk();
        }
    }
    void PlaceRandomPrefabs(float[,] heightMap) {
        prefabPositions.Clear();

        foreach (GameObject prefab in prefabTypes) {
            for (int i = 0; i < numberOfPrefabsToCreate; i++) {
                Vector3 position;
                bool validPosition;

                do {
                    validPosition = true;
                    float x = UnityEngine.Random.Range(-mapSize / 2 + safeZone, mapSize / 2 - safeZone);
                    float z = UnityEngine.Random.Range(-mapSize / 2 + safeZone, mapSize / 2 - safeZone);
                    position = new Vector3(x, 0, z);

                    if (IsTooCloseToOtherPrefabs(position, prefabPositions, safeZone)) {
                        validPosition = false;
                        continue;
                    }

                    prefabPositions.Add(position); // Stocker la position globale
                    Instantiate(prefab, position, Quaternion.identity);
                } while (!validPosition);
            }
        }
    }

    bool IsTooCloseToOtherPrefabs(Vector3 position, List<Vector3> placedPrefabs, float minDistance) {
        foreach (var otherPos in placedPrefabs) {
            if (Vector3.Distance(position, otherPos) < minDistance) {
                return true;
            }
        }
        return false;
    }

    private bool IsSpecialPrefabPosition(Vector2 position) {
        Vector3[] cornerPositions = {
            new Vector3(-715, 0, -730),
            new Vector3(-715, 0, 730),
            new Vector3(715, 0, -730),
            new Vector3(715, 0, 730)
        };

        foreach (Vector3 cornerPos in cornerPositions) {
            if (Vector2.Distance(new Vector2(cornerPos.x, cornerPos.z), position) < safeZone) {
                return true;
            }
        }
        return false;
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
        Vector2[] specialPositions = {
            new Vector2(720, 720),
            new Vector2(720, -720),
            new Vector2(-720, -720),
            new Vector2(-720, 720)
        };

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

        bool IsPositionInChunk(Vector3 localPosition) {
            // Vérifier si la position est dans le chunk
            return bounds.Contains(new Vector3(localPosition.x, 0, localPosition.z));
        }

        void OnMapDataReceived(MapData mapData) {
            this.mapdata = mapData;
            mapDataReceived = true;

            if (IsSpecialPosition(position)) {
                Debug.Log(position);
                AdjustChunkHeight(this.mapdata.heightMap, 0.5f);
            }
            RecalculateColorMap();

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, generator.mapChunckSize, generator.mapChunckSize);
            meshRenderer.material.mainTexture = texture;
            UpdateMesh();
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

        private void RecalculateColorMap() {
            for (int y = 0; y < mapdata.heightMap.GetLength(0); y++) {
                for (int x = 0; x < mapdata.heightMap.GetLength(1); x++) {
                    float height = mapdata.heightMap[x, y];
                    Color color = ChooseColorBasedOnHeight(height);
                    mapdata.colorMap[y * generator.mapChunckSize + x] = color;
                }
            }
        }

        
        private Color ChooseColorBasedOnHeight(float height) {
            foreach (var region in _generator.regions) {
                if (height <= region.height) {
                    return region.colour;
                }
            }
            return Color.white;
        }

        private void UpdateMesh() {
            _generator.RequestMeshData(mapdata, previousLOD, (meshData) => {
                meshFilter.mesh = meshData.CreateMesh();
            });
        }

        private bool IsSpecialPosition(Vector2 position) {
            foreach (var specialPosition in specialPositions) {
                if (position == specialPosition) {
                    return true;
                }
            }
            return false;
        }
        private void AdjustChunkHeight(float[,] heightMap, float newHeight) {
            int flatZoneSize = 90;

            int centerX = heightMap.GetLength(0) / 2;
            int centerY = heightMap.GetLength(1) / 2;

            int startX = centerX - flatZoneSize / 2;
            int startY = centerY - flatZoneSize / 2;
            for (int y = startY; y < startY + flatZoneSize; y++) {
                for (int x = startX; x < startX + flatZoneSize; x++) {
                    if (x >= 0 && x < heightMap.GetLength(0) && y >= 0 && y < heightMap.GetLength(1)) {
                        heightMap[y, x] = newHeight;
                    }
                }
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