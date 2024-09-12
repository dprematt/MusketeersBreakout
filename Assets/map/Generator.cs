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
using System.Linq;


public class Generator : MonoBehaviourPun
{
    public GameObject PlayerPrefab_;
    public GameObject LoadScreen_;

    public GameObject Hud_, LootHud_;
    // public enum DrawMode { map, colorMap, mesh, fallOfMap }
    public enum DrawMode { map}

    public bool autoUpdate;
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
    public Vector2 offSet;
    private SpawnWeapons SpawnWeapons_;

    float[,] colorMap = new float[mapChunkSize, mapChunkSize];
    public Vector3[] _spawnCoords = new Vector3[20];

    public float meshHeightMult;
    public AnimationCurve meshHeightCurve;
    float[,] fallOfMap;

    private bool prefabsPlaced = false;

    public TerrainType[] regions;
    public static int seed = 0;
    public PrefabType[] prefabNature;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<meshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<meshData>>();
    private Dictionary<Vector2, MapData> chunkDataMap = new Dictionary<Vector2, MapData>();
    public MapData CurrentMapData { get; private set; }
    private GameObject worldObjectsParent;
    private GameObject BiomesParent;
    public Biome[] Biomes;
    public static List<Vector3> biomesPositions = new List<Vector3>();
    public List<Vector3> guardiansSpawn = new List<Vector3>();
    public GameObject ennemyType1;
    public GameObject ennemyType2;
    public GameObject ennemyType3;

    private System.Random prng;

    public GameObject[] beachPrefabs;

    private BiomeManager biomeManager;
    private PrefabsManager prefabsManager;


    private bool hasSpawn = false;

    void Start()
    {
        SetSeedFromRoomProperties(this);
        InitializeRandom();
        BiomesParent = new GameObject("Biomes");
        BiomesParent.transform.parent = transform; 
        float randomDelay = UnityEngine.Random.Range(3f, 4f);
        worldObjectsParent = new GameObject("WorldObjectsParent");

        biomeManager = new BiomeManager(Biomes, this); 
        prefabsManager = new PrefabsManager(this, beachPrefabs, prefabNature, worldObjectsParent, prng);

        Invoke("DrawMap", randomDelay);

        prefabsManager.PlacePrefabsInBeach();
        prefabsManager.PlaceExtractionZones();
    }

    public void PlacePrefabsInChunk(Vector2 chunkCenter, float[,] heightMap, int chunkSize, System.Random prng)
    {
        InitializeRandom();
        Vector3[] extractionZonePositions = {
            new Vector3(-480, 0, -540),
            new Vector3(-480, 0, 540),
            new Vector3(480, 0, -540),
            new Vector3(480, 0, 540)
        };
        int[] prefabCounts = new int[] { 2, 5, 5, 5 };
        float chunkHalfSize = chunkSize / 2f;

        float mapLeftBorder = -2.5f * mapChunkSize + 100f;
        float mapRightBorder = 2.5f * mapChunkSize - 100f;
        float mapTopBorder = 2.5f * mapChunkSize - 100f;
        float mapBottomBorder = -2.5f * mapChunkSize + 100f;

        Dictionary<string, List<Vector3>> placedPrefabs = new Dictionary<string, List<Vector3>>();

        foreach (var prefabType in prefabNature)
        {
            placedPrefabs[prefabType.type] = new List<Vector3>();

            for (int i = 0; i < prefabCounts[Array.IndexOf(prefabNature, prefabType)]; i++)
            {
                Vector3 position = Vector3.zero;
                bool validPosition = false;
                int attempts = 0;
                int maxAttempts = 100;
                while (!validPosition && attempts < maxAttempts)
                {
                    attempts++;
                    float x = prng.Next((int)(chunkCenter.x - chunkHalfSize), (int)(chunkCenter.x + chunkHalfSize));
                    float z = prng.Next((int)(chunkCenter.y - chunkHalfSize), (int)(chunkCenter.y + chunkHalfSize));

                    int xIndex = Mathf.FloorToInt(x - (chunkCenter.x - chunkHalfSize));
                    int zIndex = Mathf.FloorToInt(z - (chunkCenter.y - chunkHalfSize));

                    if (xIndex < 0 || xIndex >= chunkSize || zIndex < 0 || zIndex >= chunkSize) continue;

                    float height = heightMap[xIndex, zIndex];
                    position = new Vector3(x, height, z);

                    bool isTooCloseToSameType = placedPrefabs[prefabType.type].Any(pos => Vector3.Distance(position, pos) < 50);
                    bool isTooCloseToExtractionZone = extractionZonePositions.Any(pos => Vector3.Distance(position, pos) < 100);
                    bool isTooCloseToBiome = biomesPositions.Any(pos => Vector3.Distance(position, pos) < 100);

                    if (!isTooCloseToSameType && !isTooCloseToExtractionZone && !isTooCloseToBiome && x >= mapLeftBorder && x <= mapRightBorder && z >= mapBottomBorder && z <= mapTopBorder)
                    {
                        if (height > 0.1f && height < 0.5f)
                        {
                            validPosition = true;
                            placedPrefabs[prefabType.type].Add(position);
                            int[] Y = new int[] { -90, 90, 180, -180, 0 };
                            int randomIndex = prng.Next(Y.Length);
                            GameObject instance = Instantiate(prefabType.prefab, position, Quaternion.identity);
                            instance.transform.Rotate(0.0f, Y[randomIndex], 0.0f);
                            instance.transform.SetParent(worldObjectsParent.transform);
                        }
                    }
                }

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning($"Failed to place prefab {prefabType.type} after {maxAttempts} attempts.");
                }
            }
        }
    }

    void PlaceBiomesGuardians()
    {
        GameObject guardiansParent = GameObject.Find("Guardians") ?? new GameObject("Guardians");

        if (PhotonNetwork.IsMasterClient) {

            foreach (Vector3 guardiansCoord in guardiansSpawn)
            {
                //GameObject guardians = PhotonNetwork.Instantiate(ennemyType1.name, guardiansCoord, Quaternion.identity);
                //guardians.transform.SetParent(guardiansParent.transform);
            }
        }
    }

    public System.Random getPRNG() {
        return prng;
    }

    public static void SetSeedFromRoomProperties(Generator instance)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mapSeed", out object seedValue))
        {
            seed = (int)seedValue;
            instance.prng = new System.Random(seed);
        }
    }

    public void InitializeRandom()
    {
        prng = new System.Random(seed);
    }

    public void DrawMap()
    {
      Vector2 topLeft = new Vector2(-2.5f * mapChunkSize, 2.5f * mapChunkSize);
        Vector2 bottomRight = new Vector2(2.5f * mapChunkSize, -2.5f * mapChunkSize);

        List<Vector2> allPlateCenters = new List<Vector2>();
        float[,] heightMap = null;

        System.Random prng = new System.Random(seed);

        for (float x = topLeft.x; x <= bottomRight.x; x += mapChunkSize)
        {
            for (float y = bottomRight.y; y <= topLeft.y; y += mapChunkSize)
            {
                var (map, plateCenters) = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, new Vector2(x, y) + offSet, normalizeMode, seed);
                allPlateCenters.AddRange(plateCenters);
                heightMap = map;

                Vector2 chunkCoord = new Vector2(x, y);
                chunkDataMap[chunkCoord] = new MapData(map, null);
            }
        }

        biomeManager.PlaceBiomesInFlatAreas(allPlateCenters, topLeft, bottomRight, prng);

        Vector2 center = Vector2.zero + offSet;
        var currentMapData = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center, normalizeMode, seed);

        selectPos(currentMapData.Item1);

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(currentMapData.Item1));
        }
        // else if (drawMode == DrawMode.colorMap)
        // {
        //     // Ne rien faire pour colorMap, car la couleur a été supprimée.
        // }
        // else if (drawMode == DrawMode.mesh)
        // {
        //     meshData meshdata = MeshGenerator.generateTerrainMesh(currentMapData.Item1, meshHeightMult, meshHeightCurve, levelOfDetail);
        // }
        // else if (drawMode == DrawMode.fallOfMap)
        // {
        //     display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOfGenerator.GenerateFallOfMap(mapChunkSize)));
        // }
        PlaceBiomesGuardians();
        CurrentMapData = new MapData(currentMapData.Item1, null);
    }

    public void selectPos(float[,] colorMap)
    {
        float minHeight = 0.2f;
        float maxHeight = 0.5f;

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
                    availableCoords.RemoveAt(randomIndex);
                }
            }
        }

        foreach (Vector3 vecteur in biomesPositions)
        {
            Vector3 pos1 = new Vector3(vecteur.x + 60, vecteur.y + 10, vecteur.z);
            Vector3 pos2 = new Vector3(vecteur.x + 60, vecteur.y + 10, vecteur.z + 5);
            Vector3 pos3 = new Vector3(vecteur.x - 60, vecteur.y + 10, vecteur.z);
            Vector3 pos4 = new Vector3(vecteur.x - 60, vecteur.y + 10, vecteur.z + 5);
            Vector3 pos5 = new Vector3(vecteur.x, vecteur.y + 10, vecteur.z - 60);
            Vector3 pos6 = new Vector3(vecteur.x + 5, vecteur.y + 10, vecteur.z - 60);
            Vector3 pos7 = new Vector3(vecteur.x, vecteur.y + 10, vecteur.z + 60);
            Vector3 pos8 = new Vector3(vecteur.x + 5, vecteur.y + 10, vecteur.z + 60);



            guardiansSpawn.Add(pos1);
            guardiansSpawn.Add(pos2);
            guardiansSpawn.Add(pos3);
            guardiansSpawn.Add(pos4);
            guardiansSpawn.Add(pos5);
            guardiansSpawn.Add(pos6);
            guardiansSpawn.Add(pos7);
            guardiansSpawn.Add(pos8);

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
        SpawnWeapons_ = GetComponent<SpawnWeapons>();
        SpawnWeapons_.InstanciateWeapons(_spawnCoords[0]);
        GameObject Player_ = PhotonNetwork.Instantiate(PlayerPrefab_.name, _spawnCoords[0], Quaternion.identity);
        Player_.GetComponent<SetupPlayer>().IsLocalPlayer();
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
    }

    MapData SkeletonGenerator(Vector2 center)
    {
        var (map, _) = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center + offSet, normalizeMode, seed);

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
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        if (enemies != null)
        {
            foreach (Enemy guardian in enemies)
            {
                guardian.biomesPositions = biomesPositions;
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
}