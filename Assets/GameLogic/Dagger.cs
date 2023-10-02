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
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 1/2;
        NewPos.y += 1;
        var dagger = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        dagger.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 1, 3);
    }
}
