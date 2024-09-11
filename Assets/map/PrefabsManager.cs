// using UnityEngine;
// using System.Collections;
// using System;
// using System.Threading;
// using System.Collections.Generic;
// using Photon.Realtime;
// using Photon.Pun;
// using PlayFab;
// using UnityEngine.UI;
// using static System.Random;
// using System.Linq;


// public class PrefabsManager : MonoBehaviourPun {
//     private generator _generator;

//     private GameObject[] beachPrefabs;

//     private PrefabType[] prefabNature;
//     public const int mapChunkSize = 241;

//     private GameObject worldObjectsParent;
//     public PrefabsManager(generator generatorInstance, GameObject[] allBeachPrefabs, PrefabType[] allPrefabsNature, GameObject worldObjectsParentInstance) {
//         this._generator = generatorInstance;
//         this.beachPrefabs = allBeachPrefabs;
//         this.prefabNature = allPrefabsNature;
//         this.worldObjectsParent =  worldObjectsParentInstance;
//     }
//     public void PlacePrefabsInChunk(Vector2 chunkCenter, float[,] heightMap, int chunkSize, System.Random prng)
//     {
//         _generator.InitializeRandom();
//         Vector3[] extractionZonePositions = {
//             new Vector3(-480, 0, -540),
//             new Vector3(-480, 0, 540),
//             new Vector3(480, 0, -540),
//             new Vector3(480, 0, 540)
//         };
//         int[] prefabCounts = new int[] { 2, 5, 5, 5 };
//         float chunkHalfSize = chunkSize / 2f;

//         float mapLeftBorder = -2.5f * mapChunkSize + 100f;
//         float mapRightBorder = 2.5f * mapChunkSize - 100f;
//         float mapTopBorder = 2.5f * mapChunkSize - 100f;
//         float mapBottomBorder = -2.5f * mapChunkSize + 100f;

//         Dictionary<string, List<Vector3>> placedPrefabs = new Dictionary<string, List<Vector3>>();

//         foreach (var prefabType in prefabNature)
//         {
//             placedPrefabs[prefabType.type] = new List<Vector3>();

//             for (int i = 0; i < prefabCounts[Array.IndexOf(prefabNature, prefabType)]; i++)
//             {
//                 Vector3 position = Vector3.zero;
//                 bool validPosition = false;
//                 int attempts = 0;
//                 int maxAttempts = 100;
//                 while (!validPosition && attempts < maxAttempts)
//                 {
//                     attempts++;
//                     float x = prng.Next((int)(chunkCenter.x - chunkHalfSize), (int)(chunkCenter.x + chunkHalfSize));
//                     float z = prng.Next((int)(chunkCenter.y - chunkHalfSize), (int)(chunkCenter.y + chunkHalfSize));

//                     int xIndex = Mathf.FloorToInt(x - (chunkCenter.x - chunkHalfSize));
//                     int zIndex = Mathf.FloorToInt(z - (chunkCenter.y - chunkHalfSize));

//                     if (xIndex < 0 || xIndex >= chunkSize || zIndex < 0 || zIndex >= chunkSize) continue;

//                     float height = heightMap[xIndex, zIndex];
//                     position = new Vector3(x, height, z);

//                     bool isTooCloseToSameType = placedPrefabs[prefabType.type].Any(pos => Vector3.Distance(position, pos) < 50);
//                     bool isTooCloseToExtractionZone = extractionZonePositions.Any(pos => Vector3.Distance(position, pos) < 100);
//                     bool isTooCloseToBiome = biomesPositions.Any(pos => Vector3.Distance(position, pos) < 100);

//                     if (!isTooCloseToSameType && !isTooCloseToExtractionZone && !isTooCloseToBiome && x >= mapLeftBorder && x <= mapRightBorder && z >= mapBottomBorder && z <= mapTopBorder)
//                     {
//                         if (height > 0.1f && height < 0.5f)
//                         {
//                             validPosition = true;
//                             placedPrefabs[prefabType.type].Add(position);
//                             int[] Y = new int[] { -90, 90, 180, -180, 0 };
//                             int randomIndex = prng.Next(Y.Length);
//                             GameObject instance = Instantiate(prefabType.prefab, position, Quaternion.identity);
//                             instance.transform.Rotate(0.0f, Y[randomIndex], 0.0f);
//                             instance.transform.SetParent(worldObjectsParent.transform);
//                         }
//                     }
//                 }

//                 if (attempts >= maxAttempts)
//                 {
//                     Debug.LogWarning($"Failed to place prefab {prefabType.type} after {maxAttempts} attempts.");
//                 }
//             }
//         }
//     }

//     public void PlaceExtractionZones()
//     {
//         int i = 1;
//         Vector3[] cornerPositions = {
//             new Vector3(-536, 4.5f, -556),
//             new Vector3(-410, 4.5f, 556),
//             new Vector3(424, 4.5f, -557),
//             new Vector3(536, 4.5f, 557)
//         };

//         foreach (Vector3 position in cornerPositions)
//         {
//             GameObject zoneInstance = Instantiate(extractionZonePrefab, position, Quaternion.identity);
//             if (i % 2 != 0)
//             {
//                 zoneInstance.transform.Rotate(0.0f, 180.0f, 0.0f);
//             }
//             i += 1;
//             Vector3 newPosition = zoneInstance.transform.position;
//             zoneInstance.transform.position = newPosition;
//         }
//     }

//     public void PlaceGameObjectsWithMinDistance(Vector2 startCoord, Vector2 endCoord, int objectCount, float minDistance, System.Random prng, GameObject prefab)
//     {

//         GameObject beachParent = GameObject.Find("BeachObjet") ?? new GameObject("BeachObjet");
//         List<Vector3> placedObjects = new List<Vector3>();

//         while (placedObjects.Count < objectCount)
//         {
            
//             float x = (float)prng.NextDouble() * (endCoord.x - startCoord.x) + startCoord.x;
//             float y = 0f;
//             float z = (float)prng.NextDouble() * (endCoord.y - startCoord.y) + startCoord.y;
        
//             Vector3 newPosition = new Vector3(x, 0.22f, z);
//             bool isFarEnough = true;
//             foreach (var placedObject in placedObjects)
//             {
//                 if (Vector3.Distance(newPosition, placedObject) < minDistance)
//                 {
//                     isFarEnough = false;
//                     break;
//                 }
//             }

//             if (isFarEnough)
//             {
//                 GameObject instance = Instantiate(prefab, newPosition, Quaternion.identity);
//                 instance.transform.SetParent(beachParent.transform);
//                 placedObjects.Add(newPosition);
//             }
//         }

//         Debug.Log($"Placés {placedObjects.Count} objets avec une distance minimale de {minDistance} unités.");
//     }

//     public void PlacePrefabsInBeach() {
//         Vector2[,] intervals = new Vector2[,] {
//             { new Vector2(-565, 555), new Vector2(555, 565) },
//             { new Vector2(-565, 555), new Vector2(-555, -565) },
//             { new Vector2(-555, -565), new Vector2(565, -555) },
//             { new Vector2(555, -565), new Vector2(565, 555) }
//         };

//         for (int i = 0; i < intervals.GetLength(0); i++) {
//             for (int j = 0; j < beachPrefabs.GetLength(0); j++) {
//                 PlaceGameObjectsWithMinDistance(intervals[i, 0],intervals[i, 1], 50, 5f, prng, beachPrefabs[j]);
//             }
//         }
//     }
// }