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
    private SpawnWeapons SpawnWeapons_;

    float[,] colorMap = new float[mapChunkSize, mapChunkSize];
    public Vector3[] _spawnCoords = new Vector3[20];

    public bool useFallOf;

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
    private object _heightMapLock = new object();
    private GameObject BiomesParent;
    public Biome[] Biomes;
    public GameObject[] objectsToPlace;
    public int[] quantitiesToPlace = new int[] { 2, 3, 3 };


    private Dictionary<string, List<Vector3>> biomeSpecificPositions = new Dictionary<string, List<Vector3>>();

    float mapSize = 1500f;
    float safeZone = 100f;
    public static List<Vector3> biomesPositions = new List<Vector3>();
    int numberOfPrefabsToCreate = 2;
    public List<Vector3> guardiansSpawn = new List<Vector3>();
    public GameObject ennemyType1;
    public GameObject ennemyType2;
    public GameObject ennemyType3;

    private System.Random prng;


    void Start()
    {
        BiomesParent = new GameObject("Biomes");
        BiomesParent.transform.parent = transform; 
        float randomDelay = UnityEngine.Random.Range(3f, 4f);
        worldObjectsParent = new GameObject("WorldObjectsParent");

        SetSeedFromRoomProperties(this);
        PlaceExtractionZones();
        InitializeRandom();
        Invoke("DrawMap", randomDelay);
        Invoke("PlaceWeaponsInBiomes", randomDelay + 1);
    }

    // void OnDrawGizmos() {
    //     float mapChunkSize = 241;
    //     float topBorder = 3.5f * mapChunkSize;
    //     float bottomBorder = -topBorder;
    //     float leftBorder = -topBorder;
    //     float rightBorder = topBorder;

    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(new Vector3(leftBorder, 0, topBorder), new Vector3(leftBorder, 0, bottomBorder));
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawLine(new Vector3(rightBorder, 0, topBorder), new Vector3(rightBorder, 0, bottomBorder));
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(new Vector3(leftBorder, 0, topBorder), new Vector3(rightBorder, 0, topBorder));
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawLine(new Vector3(leftBorder, 0, bottomBorder), new Vector3(rightBorder, 0, bottomBorder));
    // }

    public void PlacePrefabsInChunk(Vector2 chunkCenter, float[,] heightMap, int chunkSize, System.Random prng)
    {
        InitializeRandom();
        Vector3[] extractionZonePositions  = {
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
            }
        }
    }


    void PlaceBiomesGuardians()
    {
        GameObject guardiansParent = GameObject.Find("Guardians") ?? new GameObject("Guardians");

        if (PhotonNetwork.IsMasterClient) {

            foreach (Vector3 guardiansCoord in guardiansSpawn)
            {
                GameObject guardians = PhotonNetwork.Instantiate(ennemyType1.name, guardiansCoord, Quaternion.identity);
                guardians.transform.SetParent(guardiansParent.transform);
            }
        }
    }

    void PlaceBiomesInFlatAreas(List<Vector2> plateCenters, Vector2 topLeft, Vector2 bottomRight, System.Random prng)
{
        InitializeRandom();
        biomesPositions.Clear();
        biomeSpecificPositions.Clear();
        GameObject biomesParent = GameObject.Find("Biomes") ?? new GameObject("Biomes");

        foreach (Biome biome in Biomes)
        {
            List<Vector3> specificBiomePositions = new List<Vector3>();

            for (int i = 0; i < numberOfPrefabsToCreate; i++)
            {
                Vector2 center;
                bool validPosition = false;
                int attempts = 0;

                do
                {
                    if (plateCenters.Count == 0) break;
                    int index = prng.Next(0, plateCenters.Count);
                    center = plateCenters[index];
                    plateCenters.RemoveAt(index);

                    float x = center.x + prng.Next(-20, 20);
                    float z = center.y + prng.Next(-20, 20);
                    Vector3 position = new Vector3(x, 0, z);

                    float borderBuffer = 100;
                    float cornerBuffer = mapChunkSize;
                    topLeft = new Vector2(-2.5f * mapChunkSize, 2.5f * mapChunkSize);
                    bottomRight = new Vector2(2.5f * mapChunkSize, -2.5f * mapChunkSize);

                    bool isInCornerChunk = 
                        (position.x < (topLeft.x + cornerBuffer) && position.z > (topLeft.y - cornerBuffer)) ||
                        (position.x > (bottomRight.x - cornerBuffer) && position.z > (topLeft.y - cornerBuffer)) ||
                        (position.x < (topLeft.x + cornerBuffer) && position.z < (bottomRight.y + cornerBuffer)) ||
                        (position.x > (bottomRight.x - cornerBuffer) && position.z < (bottomRight.y + cornerBuffer));

                    bool isInBorderBuffer = 
                        position.x < (topLeft.x + borderBuffer) || 
                        position.x > (bottomRight.x - borderBuffer) ||
                        position.z < (bottomRight.y + borderBuffer) || 
                        position.z > (topLeft.y - borderBuffer);

                    bool isTooCloseToOtherBiomes = 
                        biomesPositions.Any(biomePos => Vector3.Distance(position, biomePos) < 200) ||
                        specificBiomePositions.Any(biomePos => Vector3.Distance(position, biomePos) < 500);

                    if (!isInBorderBuffer && !isInCornerChunk && !isTooCloseToOtherBiomes && IsPositionOnFlatArea(position, center))
                    {
                        biomesPositions.Add(position);
                        specificBiomePositions.Add(position);
                        GameObject instance = Instantiate(biome.prefab, position, Quaternion.identity);
                        instance.transform.SetParent(biomesParent.transform);
                        validPosition = true;
                    }

                    attempts++;
                } while (!validPosition && attempts < 100);
            }

            if (!biomeSpecificPositions.ContainsKey(biome.type))
            {
                biomeSpecificPositions[biome.type] = specificBiomePositions;
            }
        }
    }


    void PlaceWeaponsInBiomes(System.Random prng)
{
        InitializeRandom();
        List<Vector3> placedPositions = new List<Vector3>();

        foreach (var biomeEntry in biomeSpecificPositions)
        {
            List<Vector3> biomePositions = biomeEntry.Value;

            for (int i = 0; i < objectsToPlace.Length; i++)
            {
                GameObject objectToPlace = objectsToPlace[i];
                int quantity = quantitiesToPlace[i];

                for (int j = 0; j < quantity; j++)
                {
                    bool validPosition = false;
                    int attempts = 0;
                    int maxAttempts = 100;

                    while (!validPosition && attempts < maxAttempts)
                    {
                        attempts++;
                        int positionIndex = prng.Next(biomePositions.Count);
                        Vector3 biomePosition = biomePositions[positionIndex];
                        float x = biomePosition.x + prng.Next(-20, 20);
                        float z = biomePosition.z + prng.Next(-20, 20);
                        Vector3 position = new Vector3(x, 0.45f, z);

                        if (IsPositionValid(position, placedPositions))
                        {
                            placedPositions.Add(position);
                            GameObject instance = Instantiate(objectToPlace, position, Quaternion.identity);
                            instance.transform.SetParent(BiomesParent.transform);
                            validPosition = true;
                        }
                    }
                }
            }
        }
    }



    bool IsPositionValid(Vector3 position, List<Vector3> placedPositions)
    {
        foreach (var placedPosition in placedPositions)
        {
            if (Vector3.Distance(position, placedPosition) < 10)
            {
                return false;
            }
        }
        return true;
    }


    bool IsPositionOnFlatArea(Vector3 position, Vector2 center)
    {
        float distance = Vector2.Distance(new Vector2(position.x, position.z), center);
        float perlinNoise = Mathf.PerlinNoise(position.x * 0.1f, position.z * 0.1f) * 20f;
        return distance <= 60 + perlinNoise;
    }

    bool IsAreaFlat(Vector3 position, float[,] heightMap, int areaSize, float maxSlope)
    {
        int startX = Mathf.Max(0, Mathf.FloorToInt(position.x) - areaSize / 2);
        int startZ = Mathf.Max(0, Mathf.FloorToInt(position.z) - areaSize / 2);
        int endX = Mathf.Min(heightMap.GetLength(0) - 1, Mathf.FloorToInt(position.x) + areaSize / 2);
        int endZ = Mathf.Min(heightMap.GetLength(1) - 1, Mathf.FloorToInt(position.z) + areaSize / 2);

        float centerHeight = heightMap[Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z)];

        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                if (Mathf.Abs(heightMap[x, z] - centerHeight) > maxSlope)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void GenerateBeaches(Vector2 chunkCenter)
    {
        float normalizedWaterHeight = 0.0001f / meshHeightMult;
        int beachThickness = 50;

        // Dimensions réduites de la carte
        float mapHalfWidth = mapChunkSize * 2.5f; // 5 chunks / 2
        float mapHalfHeight = mapChunkSize * 2.5f;

        float chunkTopBorder = chunkCenter.y + (mapChunkSize / 2);
        float chunkBottomBorder = chunkCenter.y - (mapChunkSize / 2);
        float chunkRightBorder = chunkCenter.x + (mapChunkSize / 2);
        float chunkLeftBorder = chunkCenter.x - (mapChunkSize / 2);

        bool isTopBorderChunk = chunkTopBorder >= (mapHalfHeight - beachThickness) && chunkCenter.y < mapHalfHeight;
        bool isBottomBorderChunk = chunkBottomBorder <= (-mapHalfHeight + beachThickness) && chunkCenter.y > -mapHalfHeight;
        bool isLeftBorderChunk = chunkLeftBorder <= (-mapHalfWidth + beachThickness) && chunkCenter.x > -mapHalfWidth;
        bool isRightBorderChunk = chunkRightBorder >= (mapHalfWidth - beachThickness) && chunkCenter.x < mapHalfWidth;

        if (isTopBorderChunk)
            ApplyBeachToTopOrBottom(beachThickness, chunkCenter, normalizedWaterHeight, true);
        if (isBottomBorderChunk)
            ApplyBeachToTopOrBottom(beachThickness, chunkCenter, normalizedWaterHeight, false);
        if (isLeftBorderChunk)
            ApplyBeachToLeftOrRightBorder(beachThickness, chunkCenter, normalizedWaterHeight, true);
        if (isRightBorderChunk)
            ApplyBeachToLeftOrRightBorder(beachThickness, chunkCenter, normalizedWaterHeight, false);
    }



    private void ApplyBeachToTopOrBottom(int beachThickness, Vector2 chunkCenter, float normalizedWaterHeight, bool isTopBorder)
    {
        float chunkBorderStartZ = isTopBorder ? chunkCenter.y - (mapChunkSize / 2) : chunkCenter.y + (mapChunkSize / 2);
        float chunkBorderEndZ = chunkBorderStartZ + (isTopBorder ? beachThickness : -beachThickness);

        lock (_heightMapLock)
        {
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
        int startX = isLeftBorder ? 0 : mapChunkSize - beachThickness;
        int endX = isLeftBorder ? beachThickness : mapChunkSize;

        lock (_heightMapLock)
        {
            for (int x = startX; x < endX; x++)
            {
                for (int z = 0; z < mapChunkSize; z++)
                {
                    CurrentMapData.heightMap[x, z] = normalizedWaterHeight;
                }
            }
        }
    }

    public System.Random getPRNG() {
        return prng;
    }

    public static void SetSeedFromRoomProperties(generator instance)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mapSeed", out object seedValue))
        {
            seed = (int)seedValue;
            instance.prng = new System.Random(seed);
        }
    }

    private void InitializeRandom()
    {
        prng = new System.Random(seed);
    }

    public void PlaceExtractionZones()
    {
        int i = 1;
        Vector3[] cornerPositions = {
            new Vector3(-480, 0, -540),
            new Vector3(-480, 0, 540),
            new Vector3(480, 0, -540),
            new Vector3(480, 0, 540)
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
        Vector2 topLeft = new Vector2(-3.5f * mapChunkSize, 3.5f * mapChunkSize);
        Vector2 bottomRight = new Vector2(3.5f * mapChunkSize, -3.5f * mapChunkSize);

        List<Vector2> allPlateCenters = new List<Vector2>();

        for (float x = topLeft.x; x <= bottomRight.x; x += mapChunkSize)
        {
            for (float y = bottomRight.y; y <= topLeft.y; y += mapChunkSize)
            {
                var (map, plateCenters) = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, new Vector2(x, y) + offSet, normalizeMode, seed);
                allPlateCenters.AddRange(plateCenters);

                Vector2 chunkCoord = new Vector2(x, y);
                chunkDataMap[chunkCoord] = new MapData(map, colorMap);
            }
        }

        PlaceBiomesInFlatAreas(allPlateCenters, topLeft, bottomRight, prng);

        Vector2 center = Vector2.zero + offSet;
        var currentMapData = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center, normalizeMode, seed);

        selectPos(currentMapData.Item1);

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(currentMapData.Item1));
        }
        else if (drawMode == DrawMode.colorMap)
        {
            // Ne rien faire pour colorMap, car la couleur a été supprimée.
        }
        else if (drawMode == DrawMode.mesh)
        {
            meshData meshdata = MeshGenerator.generateTerrainMesh(currentMapData.Item1, meshHeightMult, meshHeightCurve, levelOfDetail);
        }
        else if (drawMode == DrawMode.fallOfMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOfGenerator.GenerateFallOfMap(mapChunkSize)));
        }
        PlaceBiomesGuardians();
        CurrentMapData = new MapData(currentMapData.Item1, colorMap);
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
        var (map, plateCenters) = Skeleton.GenerateSkeleton(mapChunkSize, mapChunkSize, scale, octaves, persistance, lacunarity, center + offSet, normalizeMode, seed);

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

[System.Serializable]
public struct Biome
{
    public string type;
    public GameObject prefab;
}

[System.Serializable]
public struct PrefabType
{
    public string type;
    public GameObject prefab;
}