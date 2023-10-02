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
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 3/2;
        NewPos.y += 1;
        var sword = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        sword.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 3, 5);
    }
}
