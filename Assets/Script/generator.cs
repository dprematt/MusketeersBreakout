using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading; 

public class generator : MonoBehaviour
{

    public enum DrawMode {map, colorMap, mesh}
    public DrawMode drawMode;
    public const int mapChunckSize = 241;
    [Range(0,6)]
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public Vector2 offSet;

    public float meshHeightMult;
    public AnimationCurve meshHeightCurve;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<meshData>> meshDataThreadQueue = new Queue<MapThreadInfo<meshData>>();


    public void displayMapInEditor() {

        MapData  mapdata = generateMap();

        DisplaySkeleton display = FindObjectOfType<DisplaySkeleton>();
        if (drawMode == DrawMode.map) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapdata.heightMap));

        } else if (drawMode == DrawMode.colorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        } else if (drawMode == DrawMode.mesh) {

            display.DrawMesh(MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve), TextureGenerator.TextureFromColorMap(mapdata.colorMap, mapChunckSize, mapChunckSize));
        }

    }
    MapData generateMap() {
        float[,] map = Skeleton.GenerateSkeleton(mapChunckSize + 2, mapChunckSize + 2, scale, octaves, persistance, lacunarity, offSet); 

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

    public void requestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            mapDataThread(callback);
        };

        new Thread  (threadStart).Start();
    }

    void mapDataThread(Action<MapData> callback) {
        MapData mapData = generateMap();

        lock(mapDataThreadQueue) {
            mapDataThreadQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }
 
    public void requestMeshData(MapData mapdata, Action<meshData> callback) {
        ThreadStart threadStart = delegate {
            meshDataThread(mapdata, callback);
        };

        new Thread (threadStart).Start();
    }

    void meshDataThread(MapData mapdata, Action<meshData> callback) {
        meshData meshdata = MeshGenerator.generateTerrainMesh(mapdata.heightMap, meshHeightMult, meshHeightCurve);
        lock(meshDataThreadQueue) {
            meshDataThreadQueue.Enqueue(new MapThreadInfo<meshData>(callback, meshdata));
        }
    }

    private void Update() {
        if (mapDataThreadQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadQueue.Count; i++) {
                MapThreadInfo<meshData> threadInfo = meshDataThreadQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    private void OnValidate()
    {
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
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData (float[,] heightMap, Color[] colorMap) {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}


[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}