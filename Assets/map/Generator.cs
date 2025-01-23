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
using Environment.Instancing;


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

    public Mesh[] instanceMeshes;
    public Material[] instanceMaterials;

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

        biomesPositions = biomeManager.getBiomesPosition();

        Invoke("DrawMap", randomDelay);

        prefabsManager.PlacePrefabsInBeach();
        prefabsManager.PlaceExtractionZones();
    }

    // private void OnDrawGizmos()
    //     {
    //         // Définir la couleur des gizmos
    //         Gizmos.color = Color.red;

    //         // Calcul des limites
    //         float mapLeftBorder = -2.0f * mapChunkSize;
    //         float mapRightBorder = 2.0f * mapChunkSize - mapChunkSize;
    //         float mapTopBorder = 2.0f * mapChunkSize - mapChunkSize;
    //         float mapBottomBorder = -2.0f * mapChunkSize;

    //         // Dessiner les lignes des bordures
    //         Vector3 topLeft = new Vector3(mapLeftBorder, 0, mapTopBorder);
    //         Vector3 topRight = new Vector3(mapRightBorder, 0, mapTopBorder);
    //         Vector3 bottomLeft = new Vector3(mapLeftBorder, 0, mapBottomBorder);
    //         Vector3 bottomRight = new Vector3(mapRightBorder, 0, mapBottomBorder);

    //         // Lignes reliant les points
    //         Gizmos.DrawLine(topLeft, topRight);
    //         Gizmos.DrawLine(topRight, bottomRight);
    //         Gizmos.DrawLine(bottomRight, bottomLeft);
    //         Gizmos.DrawLine(bottomLeft, topLeft);
    //     }

    public void PlacePrefabsInChunk(Vector2 chunkCenter, float[,] heightMap, int chunkSize, System.Random prng)
    {
        InitializeRandom();
        Vector3[] extractionZonePositions = {
            new Vector3(-536, 4.6f, -556),
            new Vector3(-418, 4.6f, 313),
            new Vector3(185, 4.6f, -554),
            new Vector3(284, 4.6f, 316)
        };
        int[] prefabCounts = new int[] { 8, 8, 8, 8, 12, 12, 12, 8, 8, 8, 8, 8, 12, 12, 12, 12, 12, 8, 8};
        float chunkHalfSize = chunkSize / 2f;

        float mapLeftBorder = -2.0f * mapChunkSize;
        float mapRightBorder = 2.0f * mapChunkSize - mapChunkSize;
        float mapTopBorder = 2.0f * mapChunkSize - mapChunkSize;
        float mapBottomBorder = -2.0f * mapChunkSize;


        Dictionary<string, List<Vector3>> placedPrefabs = new Dictionary<string, List<Vector3>>();

        foreach (var prefabType in prefabNature)
        {
            placedPrefabs[prefabType.type] = new List<Vector3>();

            for (int i = 0; i < prefabCounts[Array.IndexOf(prefabNature, prefabType)]; i++)
            {
                Vector3 position = Vector3.zero;
                bool validPosition = false;
                int attempts = 0;
                int maxAttempts = 500;
                while (!validPosition && attempts < maxAttempts)
                {
                    attempts++;
                    float x = prng.Next((int)(chunkCenter.x - chunkHalfSize), (int)(chunkCenter.x + chunkHalfSize));
                    float z = prng.Next((int)(chunkCenter.y - chunkHalfSize), (int)(chunkCenter.y + chunkHalfSize));

                    int xIndex = Mathf.FloorToInt(x - (chunkCenter.x - chunkHalfSize));
                    int zIndex = Mathf.FloorToInt(z - (chunkCenter.y - chunkHalfSize));

                    if (xIndex < 0 || xIndex >= chunkSize || zIndex < 0 || zIndex >= chunkSize) continue;

                    float height = heightMap[xIndex, zIndex];
                    position = new Vector3(x, GetPrefabHeight(prefabType.type), z);

                    bool isTooCloseToSameType = placedPrefabs[prefabType.type].Any(pos => Vector3.Distance(position, pos) < 50);
                    bool isTooCloseToExtractionZone = extractionZonePositions.Any(pos => Vector3.Distance(position, pos) < 100);
                    bool isTooCloseToBiome = biomesPositions.Any(pos => Vector3.Distance(position, pos) < 100);

                    if (!isTooCloseToSameType && !isTooCloseToExtractionZone && !isTooCloseToBiome && x >= mapLeftBorder && x <= mapRightBorder && z >= mapBottomBorder && z <= mapTopBorder)
                    {
                        if (height == 0.4f)
                        {
                            try {
                                validPosition = true;
                                placedPrefabs[prefabType.type].Add(position);
                                int[] Y = new int[] { -90, 90, 180, -180, 0 };
                                int randomIndex = prng.Next(Y.Length);
                                GameObject instance = Instantiate(prefabType.prefab, position, Quaternion.identity);
                                instance.transform.Rotate(0.0f, Y[randomIndex], 0.0f);
                                instance.transform.SetParent(worldObjectsParent.transform);
                            } catch (Exception ex)
                            {
                                Debug.LogError($"Erreur lors du placement du prefab dans le chunk: {ex.Message}");
                            }
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

    float GetPrefabHeight(string prefabType)
    {
        switch (prefabType)
        {
            case "maison": return 0.7f;
            case "buisson": return 0.7f;
            case "arbre": return 0.8f;
            case "arbre2": return 0.8f;
            case "arbre3": return 0.8f;
            case "caillou": return 0.8f;
            case "caillou2": return 0.8f;
            case "herbe1": return 0.8f;
            case "herbe2": return 0.7f;
            case "herbe3": return 0.7f;
            case "herbe4": return 0.7f;
            case "herbe5": return 0.7f;
            case "herbe6": return 0.7f;
            case "buche": return 0.3f;
            case "os": return 0.6f;
            case "feu": return 0.8f;
            default: return 0f;
        }
    }


    void PlaceBiomesGuardians()
    {
        GameObject guardiansParent = GameObject.Find("Guardians") ?? new GameObject("Guardians");
    
        if (PhotonNetwork.IsMasterClient) {

            foreach (Vector3 guardiansCoord in guardiansSpawn)
            {
                try
                {
                    GameObject guardians = PhotonNetwork.Instantiate(ennemyType1.name, guardiansCoord, Quaternion.identity);
                    guardians.transform.SetParent(guardiansParent.transform);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Erreur lors de l'instanciation du Guardian à la position {guardiansCoord}: {ex.Message}");
                }
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
        Vector2 topLeft = new Vector2(-2.0f * mapChunkSize - 150.0f, 2.0f * mapChunkSize - 50f);
        Vector2 bottomRight = new Vector2(2.0f * mapChunkSize - 50f, -2.0f * mapChunkSize - 150.0f);

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

        try {
            biomeManager.PlaceBiomesInFlatAreas(allPlateCenters, topLeft, bottomRight, prng);
        } catch (Exception ex)
        {
            Debug.LogError($"Erreur lors du placement des biomes : {ex.Message}");
        }

        Vector2 center = Vector2.zero + offSet;
        var currentMapData = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center, normalizeMode, seed);

        selectPos(currentMapData.Item1);

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(currentMapData.Item1));
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("Chunk"))
            {
                Debug.Log("Enfant trouvé avec le nom contenant 'Chunk': " + child.name);
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Debug.Log(child.name + " got a mesh");
                    child.gameObject.SetActive(false);
                    MeshInstancesBehaviour meshInstancesBehaviour = child.gameObject.AddComponent<MeshInstancesBehaviour>();

                    meshInstancesBehaviour.Density = 1f;
                    meshInstancesBehaviour.InstanceConfigurations = GenerateInstanceConfigurations(instanceMeshes, instanceMaterials);

                    child.gameObject.SetActive(true);
                }
            }
        }

        PlaceBiomesGuardians();
        CurrentMapData = new MapData(currentMapData.Item1, null);
    }

    private InstanceConfiguration[] GenerateInstanceConfigurations(Mesh[] _instanceMeshes, Material[] _instanceMaterials)
    {
        InstanceConfiguration[] configurations = new InstanceConfiguration[_instanceMeshes.Length];
        for (int i = 0; i < _instanceMeshes.Length; i++)
        {
            configurations[i] = new InstanceConfiguration
            {
                Mesh = _instanceMeshes[i],
                Material = _instanceMaterials[i],
                Probability = i == 1 ? 100 : 1,
                Scale = 0.5f,
                NormalOffset = 0
            };
        }
        return configurations;
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
            Vector3[] offsets = {
                new Vector3(60, 10, 0),
                new Vector3(60, 10, 5),
                new Vector3(-60, 10, 0),
                new Vector3(-60, 10, 5),
                new Vector3(0, 10, -60),
                new Vector3(5, 10, -60),
                new Vector3(0, 10, 60),
                new Vector3(5, 10, 60)
            };

            foreach (var offset in offsets)
            {
                guardiansSpawn.Add(vecteur + offset);
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
        try {
            SpawnWeapons_ = GetComponent<SpawnWeapons>();
            SpawnWeapons_.InstanciateWeapons(_spawnCoords[0]);
            GameObject Player_ = PhotonNetwork.Instantiate(PlayerPrefab_.name, _spawnCoords[0], Quaternion.identity);
            Player_.GetComponent<SetupPlayer>().IsLocalPlayer();
            LoadScreen_.SetActive(false);
            Hud_.SetActive(true);
            LootHud_.SetActive(true);
        } catch (Exception ex)
        {
            Debug.LogError($"Erreur lors du placement du joueur : {ex.Message}");
        }
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
        try {
            var (map, _) = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center + offSet, normalizeMode, seed);

            CurrentMapData = new MapData(map, colorMap);

            return CurrentMapData; 
        } catch (Exception ex) {
            Debug.LogError($"Erreur lors de la génération de la map: {ex.Message}");
            return default(MapData);
        }
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