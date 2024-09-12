
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallBack;

        Generator _generator;

        public LODMesh(int lod, System.Action updateCallback, Generator generatorInstance)
        {
            this._generator = generatorInstance;
            this.lod = lod;
            this.updateCallBack = updateCallback;
        }

        void OnMeshDataReceived(meshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;
            updateCallBack();
        }

        public void RequestMesh(MapData mapdata)
        {
            hasRequestedMesh = true;
            _generator.RequestMeshData(mapdata, lod, OnMeshDataReceived);
        }
    }
