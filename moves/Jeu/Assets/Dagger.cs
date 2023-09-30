using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;
    private float Range;
    private float Damage;

    public void SpawnWeaponProx()
    { 
        var dagger = Instantiate(weaponPrefab, weaponSpawnPoint.position, weaponSpawnPoint.rotation);
        dagger.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 1, 3);
    }
}
