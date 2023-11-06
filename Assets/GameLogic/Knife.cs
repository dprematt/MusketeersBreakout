using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Knife : Weapons
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;
    public Sprite _Image = null;

    public override Sprite Image
    {
        get { return _Image; }
    }

    private void Start()
    {
        LifeTime = 10;
    }

    public override void OnPickup()
    {
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Knife"; }
    }

    public void SpawnWeaponProx()
    { 
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 1/2;
        NewPos.y += 1;
        var knife = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        knife.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 1, 2);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWeaponProx();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
        }
    }
}
