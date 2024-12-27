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


public class PrefabsManager : MonoBehaviourPun {
    private Generator _generator;

    private GameObject[] beachPrefabs;

    private PrefabType[] prefabNature;
    public const int mapChunkSize = 241;

    private System.Random prng;

    private GameObject worldObjectsParent;

    public PrefabsManager(Generator generatorInstance, GameObject[] allBeachPrefabs, PrefabType[] allPrefabsNature, GameObject worldObjectsParentInstance, System.Random prngInstance) {
        this._generator = generatorInstance;
        this.beachPrefabs = allBeachPrefabs;
        // this.prefabNature = allPrefabsNature;
        this.worldObjectsParent =  worldObjectsParentInstance;
        this.prng = prngInstance;
    }

    public void PlaceExtractionZones()
    {
        GameObject extractionZonePrefab = Resources.Load<GameObject>("ExtractionZone");
        int i = 1;
        Vector3[] cornerPositions = {
            new Vector3(-536, 4.6f, -556),
            new Vector3(-418, 4.6f, 313),
            new Vector3(185, 4.6f, -554),
            new Vector3(284, 4.6f, 316)
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
            zoneInstance.transform.position = newPosition;
        }
    }

    public void PlaceGameObjectsWithMinDistance(Vector2 startCoord, Vector2 endCoord, int objectCount, float minDistance, System.Random prng, GameObject prefab)
    {

        GameObject beachParent = GameObject.Find("BeachObjet") ?? new GameObject("BeachObjet");
        List<Vector3> placedObjects = new List<Vector3>();

        while (placedObjects.Count < objectCount)
        {
            
            float x = (float)prng.NextDouble() * (endCoord.x - startCoord.x) + startCoord.x;
            float z = (float)prng.NextDouble() * (endCoord.y - startCoord.y) + startCoord.y;
        
            Vector3 newPosition = new Vector3(x, 0.22f, z);
            bool isFarEnough = true;
            foreach (var placedObject in placedObjects)
            {
                if (Vector3.Distance(newPosition, placedObject) < minDistance)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                GameObject instance = Instantiate(prefab, newPosition, Quaternion.identity);
                instance.transform.SetParent(beachParent.transform);
                placedObjects.Add(newPosition);
            }
        }

        Debug.Log($"Placés {placedObjects.Count} objets avec une distance minimale de {minDistance} unités.");
    }

    public void PlacePrefabsInBeach() {
        Vector2[,] intervals = new Vector2[,] {
            { new Vector2(-565, 310), new Vector2(315, 325) },
            { new Vector2(-565, 325), new Vector2(-555, -565) },
            { new Vector2(-555, -565), new Vector2(325, -555) },
            { new Vector2(310, -565), new Vector2(325, 330) }
        };

        for (int i = 0; i < intervals.GetLength(0); i++) {
            for (int j = 0; j < beachPrefabs.GetLength(0); j++) {
                PlaceGameObjectsWithMinDistance(intervals[i, 0],intervals[i, 1], 50, 5f, prng, beachPrefabs[j]);
            }
        }
    }
}