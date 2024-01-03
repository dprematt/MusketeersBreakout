using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
public class generator : MonoBehaviour
{

    public GameObject extractionZonePrefab;

    public enum DrawMode {map, colorMap, mesh, fallOfMap}
    public DrawMode drawMode;
    public const int mapChunckSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public Skeleton.NormalizeMode normalizeMode;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offSet;

    public bool useFallOf;

    
    public float meshHeightMult;
    public AnimationCurve meshHeightCurve;
    float[,] fallOfMap;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<meshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<meshData>>();

    private void Start() {
        PlaceExtractionZones();
    }

    public void PlaceExtractionZones() {
        int i = 1;
        Vector3[] cornerPositions = {
            new Vector3(-715, 0, -730),
            new Vector3(-715, 0, 730),
            new Vector3(715, 0, -730),
            new Vector3(715, 0, 730)
        };

        foreach (Vector3 position in cornerPositions) {
            GameObject zoneInstance = Instantiate(extractionZonePrefab, position, Quaternion.identity);
            if (i % 2 != 0) {
                zoneInstance.transform.Rotate(0.0f, 180.0f, 0.0f);
            }
            i += 1;
            Vector3 newPosition = zoneInstance.transform.position;
            newPosition.y = 2;
            zoneInstance.transform.position = newPosition; 
        }
    }
    public void DrawMap() {
        MapData mapdata = SkeletonGenerator(Vector2.zero);
        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map) {

            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapdata.heightMap));

        } else if (drawMode == DrawMode.colorMap) {

            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        } else if (drawMode == DrawMode.mesh) {
            display.DrawMesh(MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        } else if (drawMode == DrawMode.fallOfMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOfGenerator.GenerateFallOfMap(mapChunckSize)));
        }
    }
    
    MapData SkeletonGenerator(Vector2 center) {
        float[,] map = Skeleton.GenerateSkeleton(mapChunckSize + 2, mapChunckSize + 2, scale, octaves, persistance, lacunarity, center + offSet, normalizeMode); 

        Color[] colorMap = new Color[mapChunckSize * mapChunckSize];

        for (int i = 0; i < mapChunckSize; i++) {

            for (int j = 0; j < mapChunckSize; j++) {

                float curHeight = map[j,i];

                for (int k = 0; k < regions.Length; k++) {

                    if (curHeight >= regions[k].height) {

                        colorMap[i * mapChunckSize + j]  = regions[k].colour;
                        
                    }
                    else {
                        break;
                    }

                }
            }
        }
        return new MapData(map, colorMap);
    }

Vector3[] GetSpawnCoords(float[,] map)
{
    List<Vector3> spawnCoords = new List<Vector3>();

    for (int i = 0; i < map.GetLength(0); i++)
    {
        for (int j = 0; j < map.GetLength(1); j++)
        {
            float height = map[j, i];

            if (height >= 0 && height <= 0.5f && enoughSpace(spawnCoords, i, j, 100))
            {
                Vector3 coord = new Vector3(i, j, height);
                spawnCoords.Add(coord);

                if (spawnCoords.Count == 8)
                {
                    return spawnCoords.ToArray();
                }
            }
        }
    }

    return spawnCoords.ToArray();
}

    bool enoughSpace(List<Vector3> coords, int x, int y, float distanceMinimale)
    {
        foreach (Vector3 existingCoord in coords)
        {  
            float distance = distanceEuclidienne(existingCoord.x, existingCoord.y, x, y);

            if (distance < distanceMinimale)
            {
                return false;
            }
        }

        return true;
    }

    float distanceEuclidienne(float x1, float y1, float x2, float y2)
    {
        float dx = x1 - x2;
        float dy = y1 - y2;

        return (float)Math.Sqrt(dx * dx + dy * dy);
    }


    public void RequestMapData(Vector2  center, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback) {
        MapData mapdata = SkeletonGenerator(center);
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapdata));
        }
    }

    public void RequestMeshData(MapData mapdata, int lod, Action<meshData> callback) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapdata, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapdata, int lod, Action<meshData> callback) {
        meshData meshdata = MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<meshData>(callback, meshdata));
        }
    }


    private void Update() {
        if (mapDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<meshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    private void OnValidate() {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
    struct MapThreadInfo<T> {
        public  readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}

public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData (float[,] heightMap, Color[] colorMap) {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}