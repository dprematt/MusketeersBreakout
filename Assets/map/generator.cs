using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using PlayFab;
using UnityEngine.UI;
using static System.Random;

public class generator : MonoBehaviourPun
{
    public GameObject extractionZonePrefab;
    public GameObject PlayerPrefab_;
    public GameObject LoadScreen_;

    public GameObject Hud_, LootHud_;

    public enum DrawMode { map, colorMap, mesh, fallOfMap }

    public DrawMode drawMode;
    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    public Skeleton.NormalizeMode normalizeMode;
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public Vector2 offSet;

    float[,] colorMap = new float[mapChunkSize, mapChunkSize];
    public Vector3[] _spawnCoords = new Vector3[20];


    public bool useFallOf;

    public float meshHeightMult;
    public AnimationCurve meshHeightCurve;
    float[,] fallOfMap;

    private bool prefabsPlaced = false;

    public TerrainType[] regions;
    public static int seed = 0;
    public GameObject[] prefabNature;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<meshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<meshData>>();
    private Dictionary<Vector2, MapData> chunkDataMap = new Dictionary<Vector2, MapData>();
    public MapData CurrentMapData { get; private set; }
    private GameObject worldObjectsParent;
    private object _heightMapLock = new object();

    private void Start()
    {
        float randomDelay = UnityEngine.Random.Range(3f, 4f);
        worldObjectsParent = new GameObject("WorldObjectsParent");

        SetSeedFromRoomProperties();
        PlaceExtractionZones();
        Invoke("DrawMap", randomDelay);
    }

    void OnDrawGizmos() {
        // Dessine une ligne rouge au bord supérieur de la plage.
        float mapChunkSize = 241;
        float topBorder = 3.5f * mapChunkSize;
        float bottomBorder = -topBorder;
        float leftBorder = -topBorder;
        float rightBorder = topBorder;

        Gizmos.color = Color.yellow;
        // Gauche de la carte
        Gizmos.DrawLine(new Vector3(leftBorder, 0, topBorder), new Vector3(leftBorder, 0, bottomBorder));
        Gizmos.color = Color.green;
        // Droit de la carte
        Gizmos.DrawLine(new Vector3(rightBorder, 0, topBorder), new Vector3(rightBorder, 0, bottomBorder));
    
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBorder, 0, topBorder), new Vector3(rightBorder, 0, topBorder));

        // Dessine une ligne bleue au bord inférieur de la plage.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(leftBorder, 0, bottomBorder), new Vector3(rightBorder, 0, bottomBorder));
    }

    public void PlacePrefabsInChunk(Vector2 chunkCenter, float[,] heightMap, int chunkSize)
    {
        int[] prefabCounts = new int[] {4, 10, 10, 10};
        float chunkHalfSize = chunkSize / 2f;

        float mapLeftBorder = -3.5f * mapChunkSize;
        float mapRightBorder = 3.5f * mapChunkSize;
        float mapTopBorder = 3.5f * mapChunkSize;
        float mapBottomBorder = -3.5f * mapChunkSize;


        for (int prefabIndex = 0; prefabIndex < prefabNature.Length; prefabIndex++)
        {
            GameObject prefab = prefabNature[prefabIndex];
            for (int i = 0; i < prefabCounts[prefabIndex]; i++) {
                Vector3 position = Vector3.zero;
                bool validPosition = false;
                int attempts = 0;
                int maxAttempts = 100;
                while (!validPosition && attempts < maxAttempts)
                {
                    attempts++;
                    float x = UnityEngine.Random.Range(chunkCenter.x - chunkHalfSize, chunkCenter.x + chunkHalfSize);
                    float z = UnityEngine.Random.Range(chunkCenter.y - chunkHalfSize, chunkCenter.y + chunkHalfSize);

                    int xIndex = Mathf.FloorToInt(x - (chunkCenter.x - chunkHalfSize));
                    int zIndex = Mathf.FloorToInt(z - (chunkCenter.y - chunkHalfSize));

                    if (xIndex < 0 || xIndex >= chunkSize || zIndex < 0 || zIndex >= chunkSize) continue;

                    float height = heightMap[xIndex, zIndex];

                    if (x >= mapLeftBorder && x <= mapRightBorder && z >= mapBottomBorder && z <= mapTopBorder) {
                        if (height > 0.1f && height < 0.5f)
                        {
                            position = new Vector3(x, height, z);
                            validPosition = true;
                            int[] Y = new int[] {-90, 90, 180, -180, 0};
                            int randomIndex = UnityEngine.Random.Range(0, Y.Length);
                            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                            instance.transform.Rotate(0.0f, Y[randomIndex], 0.0f);
                            instance.transform.SetParent(worldObjectsParent.transform);
                        }
                    }
                }
            }
        }
    }



    private void GenerateBeaches(Vector2 chunkCenter)
    {
        float normalizedWaterHeight = 0.0001f / meshHeightMult;
        int beachThickness = 50;

        float mapHalfSize = mapChunkSize * 7 / 2;

        float leftMapBorder = -mapHalfSize;
        float rightMapBorder = mapHalfSize;

        float chunkTopBorder = chunkCenter.y + (mapChunkSize / 2);
        float chunkBottomBorder = chunkCenter.y - (mapChunkSize / 2);
        float chunkRightBorder = chunkCenter.x + (mapChunkSize / 2);
        float chunkLeftBorder = chunkCenter.x - (mapChunkSize / 2);

        bool isTopBorderChunk = chunkTopBorder > 720;
        bool isBottomBorderChunk = chunkBottomBorder < -720;
        bool isLeftBorderChunk = chunkLeftBorder < leftMapBorder + beachThickness;
        bool isRightBorderChunk = chunkRightBorder > rightMapBorder - beachThickness;

        if (isTopBorderChunk)
        {
            ApplyBeachToTopOrBottom(beachThickness, chunkCenter, normalizedWaterHeight, true);
        }


        if (isBottomBorderChunk)
        {
            ApplyBeachToTopOrBottom(beachThickness, chunkCenter, normalizedWaterHeight, false);
        }

        if (isLeftBorderChunk)
        {
            ApplyBeachToLeftOrRightBorder(beachThickness, chunkCenter, normalizedWaterHeight, true);
        }
        if (isRightBorderChunk)
        {
            ApplyBeachToLeftOrRightBorder(beachThickness, chunkCenter, normalizedWaterHeight, false);
        }
    }

    private void ApplyBeachToTopOrBottom(int beachThickness, Vector2 chunkCenter, float normalizedWaterHeight, bool isTopBorder)
    {
        float chunkBorderStartZ = isTopBorder ? chunkCenter.y - (mapChunkSize / 2) : chunkCenter.y + (mapChunkSize / 2);
        float chunkBorderEndZ = chunkBorderStartZ + (isTopBorder ? beachThickness : -beachThickness);

        lock (_heightMapLock) {
            for (int x = 0; x < mapChunkSize; x++)
            {
                for (int z = 0; z < mapChunkSize; z++)
                {
                    float worldZ = chunkCenter.y - (mapChunkSize / 2) + z;

                    if (isTopBorder ? (worldZ >= chunkBorderStartZ && worldZ <= chunkBorderEndZ) :
                                    (worldZ <= chunkBorderStartZ && worldZ >= chunkBorderEndZ))
                    {
                        CurrentMapData.heightMap[x, z] = normalizedWaterHeight;
                    }
                }
            }
        }
    }

    private void ApplyBeachToLeftOrRightBorder(int beachThickness, Vector2 chunkCenter, float normalizedWaterHeight, bool isLeftBorder)
    {
        // Commencer ou finir la plage selon le bord où se trouve le chunk
        int startX = isLeftBorder ? 0 : mapChunkSize - beachThickness;
        int endX = isLeftBorder ? beachThickness : mapChunkSize;
        
        lock (_heightMapLock) {
            for (int x = startX; x < endX; x++)
            {
                for (int z = 0; z < mapChunkSize; z++)
                {
                    // Appliquer la plage sur la bordure du chunk
                    CurrentMapData.heightMap[x, z] = normalizedWaterHeight;
                }
            }
        }
    }


    public static void SetSeedFromRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mapSeed", out object seedValue))
        {
            seed = (int)seedValue;
            Debug.Log($"Seed récupérée dans generator: {seed}");
        }
    }

    public void PlaceExtractionZones()
    {
        int i = 1;
        Vector3[] cornerPositions = {
            new Vector3(-715, 0, -730),
            new Vector3(-715, 0, 730),
            new Vector3(715, 0, -730),
            new Vector3(715, 0, 730)
        };

        foreach (Vector3 position in cornerPositions)
        {
            GameObject zoneInstance = Instantiate(extractionZonePrefab, position, Quaternion.identity);
            if (i % 2 != 0)
            {
                zoneInstance.transform.Rotate(0.0f, 180.0f, 0.0f);
            }
            i += 1;
            Vector3 newPosition = zoneInstance.transform.position;
            newPosition.y = 2;
            zoneInstance.transform.position = newPosition;
        }
    }

    public void DrawMap()
    {
        MapData mapdata = SkeletonGenerator(Vector2.zero);
        
        selectPos(colorMap);

        // Debug.Log("TAB LENGHT = " + _spawnCoords.Length);
        // for (int i = 0; i < _spawnCoords.Length; ++i)
        //     Debug.Log("tmp[" + i + "] = " + _spawnCoords[i].ToString());

        // if (_spawnCoords.Length == 0)
        //     Debug.Log("TMP = EMPTY");



        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapdata.heightMap));
        }
        else if (drawMode == DrawMode.colorMap)
        {
            // Ne rien faire pour colorMap, car la couleur a �t� supprim�e.
        }
        else if (drawMode == DrawMode.mesh)
        {
            meshData meshdata = MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve, levelOfDetail);
        }
        else if (drawMode == DrawMode.fallOfMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOfGenerator.GenerateFallOfMap(mapChunkSize)));
        }
    }


    public void selectPos(float[,] colorMap)
    {
        float minHeight = 0.2f;
        float maxHeight = 0.5f;
        Debug.Log("SelectPos");

        if (PhotonNetwork.IsMasterClient)
        {
            List<Vector2Int> availableCoords = new List<Vector2Int>();

            for (int i = 0; i < mapChunkSize; i++)
            {
                for (int j = 0; j < mapChunkSize; j++)
                {
                    float curHeight = colorMap[j, i];

                    if (curHeight >= minHeight && curHeight <= maxHeight)
                    {
                        availableCoords.Add(new Vector2Int(j, i));
                    }
                }
            }

            if (availableCoords.Count >= 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableCoords.Count);
                    Vector2Int selectedCoord = availableCoords[randomIndex];

                    float randomHeight = colorMap[selectedCoord.y, selectedCoord.x];

                    randomHeight = Mathf.Clamp(randomHeight, minHeight, maxHeight) + 10;

                    _spawnCoords[i] = new Vector3(selectedCoord.x, randomHeight, selectedCoord.y);
                    // Debug.Log("_spawnCoords : " + _spawnCoords[i]);

                    availableCoords.RemoveAt(randomIndex);
                }
            }
            else
            {
                Debug.LogError("Pas assez de coordonnées disponibles.");
            }
        }
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GetCoords", RpcTarget.All, _spawnCoords);
        }
        float randomDelay = UnityEngine.Random.Range(1f, 2f);

        Invoke("SpawnFunc", randomDelay);

    }

    private void SpawnFunc()
    {
        GameObject Player_ = PhotonNetwork.Instantiate(PlayerPrefab_.name, _spawnCoords[0], Quaternion.identity);
        Debug.Log("Nom de l'instance PlayFab : " + PlayFabSettings.TitleId + " Spawn aux coordonnées : x = " + _spawnCoords[0].x + " y = " + _spawnCoords[0].y + " z = " + _spawnCoords[0].z);
        Player_.GetComponent<SetupPlayer>().IsLocalPlayer();
        //photonView.RPC("DeleteCoords", RpcTarget.All);
        LoadScreen_.SetActive(false);
        Hud_.SetActive(true);
        LootHud_.SetActive(true);
    }

    [PunRPC]
    private void GetCoords(Vector3[] spawnCoords)
    {
        _spawnCoords = spawnCoords;
    }

    [PunRPC]
    private void DeleteCoords()
    {
        List<Vector3> spawnCoordsList = new List<Vector3>(_spawnCoords);
        if (spawnCoordsList.Count > 0)
        {
            spawnCoordsList.RemoveAt(0);
            _spawnCoords = spawnCoordsList.ToArray();
        }
        else
        {
            Debug.LogWarning("La liste _spawnCoords est vide.");
        }
    }


    MapData SkeletonGenerator(Vector2 center)
    {

        float[,] map = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center + offSet, normalizeMode, seed);


        for (int i = 0; i < mapChunkSize; i++)
        {
            for (int j = 0; j < mapChunkSize; j++)
            {
                float curHeight = map[j, i];

                for (int k = 0; k < regions.Length; k++)
                {
                    if (curHeight >= regions[k].height)
                    {
                        colorMap[j, i] = curHeight;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        CurrentMapData = new MapData(map, colorMap);
        GenerateBeaches(center);

        return CurrentMapData;
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapdata = SkeletonGenerator(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapdata));
        }
    }

    public void RequestMeshData(MapData mapdata, int lod, Action<meshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapdata, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapdata, int lod, Action<meshData> callback)
    {
        meshData meshdata = MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<meshData>(callback, meshdata));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<meshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly float[,] colorMap;

    public MapData(float[,] heightMap, float[,] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
