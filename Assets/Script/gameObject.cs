using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour {

    public const float viewDistance = 500;
    public Transform viewer;

    public Material mapMaterial;
    static generator _generator;

    public static Vector2 viewerPosition;
    int chunckSize;

    int chunckVisibleDist;

    Dictionary<Vector2, Chunck> chunckDic = new Dictionary<Vector2, Chunck>();
    List<Chunck> chuncksLastUpdate = new List<Chunck>();

    private void Start() {
        _generator = FindObjectOfType<generator>();
        chunckSize = generator.mapChunckSize - 1;
        chunckVisibleDist = Mathf.RoundToInt(viewDistance / chunckSize);
        updateChunck();
    }

    private void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        updateChunck();
    }


    void updateChunck() {

        for (int i = 0; i < chuncksLastUpdate.Count; i ++) {
            chuncksLastUpdate[i].setVisible(false);
        }

        chuncksLastUpdate.Clear();

        int currentChunckX = Mathf.RoundToInt(viewerPosition.x / chunckSize);
        int currentChunckY = Mathf.RoundToInt(viewerPosition.y / chunckSize);

        for (int offsetY = -chunckVisibleDist; offsetY <= chunckVisibleDist; offsetY ++) {
            for (int offsetX = -chunckVisibleDist; offsetX <= chunckVisibleDist; offsetX ++) {
                Vector2 chunckCoords = new Vector2(currentChunckX + offsetX, currentChunckY + offsetY);

                if (chunckDic.ContainsKey(chunckCoords)) {
                    chunckDic[chunckCoords].UpdateChunck();
                    if (chunckDic[chunckCoords].isVisible()) {
                        chuncksLastUpdate.Add(chunckDic[chunckCoords]);
                    }
                } else {
                    chunckDic.Add(chunckCoords, new Chunck(chunckCoords, chunckSize, transform, mapMaterial));
                }
            }
        }
    }

    public class Chunck {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        public Chunck(Vector2 coord, int size, Transform parent, Material material)  {
            position = coord * size;
            bounds =  new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);


            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            meshRenderer.material = material;
            setVisible(false);
            _generator.requestMapData(onMapDataReceived);

        }

        void onMapDataReceived(MapData mapdata) {

            _generator.requestMeshData(mapdata, OnMeshDataReceived);
        }

        void OnMeshDataReceived(meshData _meshData) {
            meshFilter.mesh = _meshData.CreateMesh();
        }

        public void UpdateChunck() {
            float viewerDistanceFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromEdge <= viewDistance;
            setVisible(visible);
        }

        public void setVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool isVisible() {
            return meshObject.activeSelf;
        }
    }
}