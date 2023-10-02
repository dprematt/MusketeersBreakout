using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Halberd : WeaponsProx
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;
    private float Range;
    private float Damage;

    public void SpawnWeaponProx()
    {
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 4/2;
        NewPos.y += 1;
        var halberd = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
    }
}
