using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;
    private float Range;
    private float Damage;

    public void SpawnWeaponProx()
    { 
        var sword = Instantiate(weaponPrefab, weaponSpawnPoint.position, weaponSpawnPoint.rotation);
        sword.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 3, 5);
    }
}
