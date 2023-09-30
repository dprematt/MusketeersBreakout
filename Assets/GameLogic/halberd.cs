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

    // Start is called before the first frame update
    void Start()
    {
        //Damage = 2; ca marche pas de mettre la valeur ici
        //Range = 25; ca marche pas de mettre la valeur ici
    }

    public void SpawnWeaponProx()
    {
        var halberd = Instantiate(weaponPrefab, weaponSpawnPoint.position, weaponSpawnPoint.rotation);
        //knife.GetComponent<Rigidbody>();
        halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
    }
}
