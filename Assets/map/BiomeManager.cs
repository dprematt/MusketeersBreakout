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

public class BiomeManager : MonoBehaviourPun {
    private Biome[] Biomes;

    private Generator _generator;

    public static List<Vector3> biomesPositions = new List<Vector3>();

    private Dictionary<string, List<Vector3>> biomeSpecificPositions = new Dictionary<string, List<Vector3>>();


    public BiomeManager(Biome[] allBiomes, Generator generatorInstance) {
        this.Biomes = allBiomes;
        this._generator = generatorInstance;
    }

    public List<Vector3> getBiomesPosition() {
        return biomesPositions;
    }

    public void PlaceBiomesInFlatAreas(List<Vector2> plateCenters, Vector2 topLeft, Vector2 bottomRight, System.Random prng)
    {
        
        _generator.InitializeRandom();
        biomesPositions.Clear();
        biomeSpecificPositions.Clear();

        foreach (Biome biome in Biomes)
        {
            biomeSpecificPositions.Add(biome.type, new List<Vector3>());
        }

        GameObject biomesParent = GameObject.Find("Biomes") ?? new GameObject("Biomes");

        if (plateCenters.Count == 0)
        {
            Debug.LogError("Plus de zones plates disponibles pour le placement des biomes.");
            return;
        }

        if (plateCenters.Count < Biomes.Length * 2)
        {
            Debug.LogWarning("Pas assez de zones plates pour placer tous les biomes.");
            return;
        }

        if (biomesParent.transform.childCount > 0)
        {
            foreach (Transform child in biomesParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        Dictionary<string, int> biomeCount = new Dictionary<string, int>();
        foreach (Biome biome in Biomes)
        {
            biomeCount.Add(biome.type, 0);
        }

        int totalBiomesNeeded = Biomes.Length * 2;
        int biomesPlaced = 0;
        float chunkSize = 241f;
        float borderBuffer = 120f;

        int maxAttempts = 2000;
        int attemptCount = 0;

        while (biomesPlaced < totalBiomesNeeded && plateCenters.Count > 0)
        {
            foreach (Biome biome in Biomes)
            {
                if (biomeCount[biome.type] >= 2)
                {
                    continue;
                }

                bool biomePlaced = false;

                while (!biomePlaced && plateCenters.Count > 0 && attemptCount < maxAttempts)
                {
                    attemptCount++;

                    if (attemptCount >= maxAttempts)
                    {
                        Debug.LogError("Nombre maximum d'essais atteint, arrêt du placement.");
                        return;
                    }

                    int index = prng.Next(0, plateCenters.Count);
                    Vector2 center = plateCenters[index];
                    plateCenters.RemoveAt(index);

                    float biomeRadius = 150f;
                    bool isOutOfBounds = 
                        (center.x - biomeRadius < topLeft.x + borderBuffer || center.x + biomeRadius > bottomRight.x - borderBuffer ||
                        center.y - biomeRadius < bottomRight.y + borderBuffer || center.y + biomeRadius > topLeft.y - borderBuffer);

                    if (isOutOfBounds)
                    {
                        Debug.LogWarning($"Le biome à {center} est partiellement hors des limites de la carte.");
                        continue;
                    }

                    bool isInCorner = (center.x < topLeft.x + chunkSize && center.y > topLeft.y - chunkSize) || 
                                    (center.x > bottomRight.x - chunkSize && center.y < bottomRight.y + chunkSize);

                    if (isInCorner)
                    {
                        Debug.LogWarning($"La zone à {center} est proche d'un coin.");
                        continue;
                    }

                    float x = center.x;
                    float y = GetBiomeHeight(biome.type);
                    float z = center.y;
                    Vector3 position = new Vector3(x, y, z);

                    bool isTooCloseToOtherBiomes = biomesPositions.Any(biomePos => (position - biomePos).sqrMagnitude < 120 * 120);
                    bool isTooCloseToSameBiomes = biomeSpecificPositions[biome.type].Any(biomePos => (position - biomePos).sqrMagnitude < 200 * 200);

                    if (!isTooCloseToOtherBiomes && !isTooCloseToSameBiomes)
                    {
                        try {
                            biomesPositions.Add(position);
                            biomeSpecificPositions[biome.type].Add(position);

                            GameObject instance = Instantiate(biome.prefab, position, Quaternion.identity);
                            DropWeaponsInChest(instance, prng);
                            instance.transform.SetParent(biomesParent.transform);

                            biomeCount[biome.type] += 1;
                            biomesPlaced += 1;
                            biomePlaced = true;
                        } catch (Exception ex) {
                            Debug.LogError($"Erreur lors du placement du biome de type {biome.type} : {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"La position {position} est trop proche d'autres biomes ou du même type.");
                    }
                }
            }
        }
        Debug.Log("Fin de PlaceBiomesInFlatAreas");
    }

    float GetBiomeHeight(string biomeType)
    {
        switch (biomeType)
        {
            case "désert": return 0.7f;
            case "neige": return 0.7f;
            case "médieval": return 0.8f;
            case "jungle": return -0.5f;
            case "village": return 0.8f;
            case "loot": return 0.8f;
            default: return 0f;
        }
    }

    public void DropWeaponsInChest(GameObject prefab, System.Random prng)
    {
        // string[] allWeapons = { "Sword", "Gun", "Dagger", "Shield" };
        string[] allWeapons = { "Sword", "Gun", "Dagger"};
        string lootPrefabName = "Prefabs/Loot";
        Vector3[] positions = new Vector3[0];

        switch(prefab.name) {
            case("LootZone(Clone)"):
                positions = new Vector3[] {
                    new Vector3(-12, 0, 7),
                    new Vector3(-10, 0, -1),
                    new Vector3(7, 0, 4),
                    new Vector3(6, 0, -14)
                };
                break;
            case("DesertBiome(Clone)"):
                positions = new Vector3[] {
                    new Vector3(-30, 0, -17),
                    new Vector3(-12, 0, 7),
                    new Vector3(8, 0, -6),
                    new Vector3(-13, 0,-29),
                    new Vector3(-27, 0, -26),
                    new Vector3(18, 0, -26),
                    new Vector3(-31, 0, 4)
                };
                break;
            case("JungleBiome(Clone)"):
                positions = new Vector3[] {
                    new Vector3(10, 1, -5),
                    new Vector3(27, 1, -16),
                    new Vector3(25, 1, 3),
                    new Vector3(13, 1, 13),
                    new Vector3(-13, 1, -27),
                    new Vector3(-3, 1, -6)
                };
                break;
            case("MedievalBiome(Clone)"):
                positions = new Vector3[] {
                    new Vector3(-113, 0, -147),
                    new Vector3(-22, 0,  -152),
                    new Vector3(-45, 0, -114),
                    new Vector3(-5, 0, -110),
                    new Vector3(-21,0 ,-52),
                    new Vector3(-138, 0, -165)
                };
                break;
            case("SnowBiome(Clone)"):
                positions = new Vector3[] {
                    new Vector3(0, 0, 3),
                    new Vector3(10, 0, -10),
                    new Vector3(-8,0,-6),
                    new Vector3(14,0,4),
                    new Vector3(22,0,-11),
                    new Vector3(1,0,10)
                };
                break;
            case("Village(Clone)"):
                positions = new Vector3[] {
                    new Vector3(-53,0,111),
                    new Vector3(-72,0,124),
                    new Vector3(-79,0,137),
                    new Vector3(-51,0,143)
                };
                break;
            default:
                break;
        }
        foreach (Vector3 pos in positions) {
            try  {
                int randomWeaponIndex = prng.Next(allWeapons.Length);
                string chosenWeapon = allWeapons[randomWeaponIndex];

                GameObject lootInstance = PhotonNetwork.Instantiate(lootPrefabName, prefab.transform.position, Quaternion.identity);

                Inventory lootInventory = lootInstance.transform.GetChild(0).GetComponentInChildren<Inventory>();
                lootInventory.DropWeapons(chosenWeapon);
                lootInventory.loot = true;

                lootInstance.transform.SetParent(prefab.transform, true);

                lootInstance.transform.localPosition = pos;            

            } catch (Exception ex) {
                Debug.LogError($"Erreur lors de la création d'arme dans le coffre : {ex.Message}");
            }
        }
    }

}
