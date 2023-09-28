using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
public class generator : MonoBehaviour
{

    public enum DrawMode {map, colorMap, mesh}
    public DrawMode drawMode;
    public const int mapChunckSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
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

    //private void awake() {
      //  fallOfMap = FallOfMapGenerator.GenerateFallOfMap(mapChunckSize);
    //}

    public void DrawMap() {
        MapData mapdata = SkeletonGenerator(Vector2.zero);

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map) {

            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapdata.heightMap));

        } else if (drawMode == DrawMode.colorMap) {

            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        } else if (drawMode == DrawMode.mesh) {
            display.DrawMesh(MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        }
    }
    MapData SkeletonGenerator(Vector2 center) {
        float[,] map = Skeleton.GenerateSkeleton(mapChunckSize, mapChunckSize, scale, octaves, persistance, lacunarity, center + offSet); 

        Color[] colorMap = new Color[mapChunckSize * mapChunckSize];

        for (int i = 0; i < mapChunckSize; i++) {

            for (int j = 0; j < mapChunckSize; j++) {

                float curHeight = map[j,i];

                for (int k = 0; k < regions.Length; k++) {

                    if (curHeight <= regions[k].height) {

                        colorMap[i * mapChunckSize + j]  = regions[k].colour;
                        break;
                    }

                }
            }
        }
        return new MapData(map, colorMap);
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