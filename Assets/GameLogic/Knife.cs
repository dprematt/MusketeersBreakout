using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Knife : WeaponsProx
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
        var knife = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        knife.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 1, 2);
        Debug.Log("knife stab launched");
    }
}
