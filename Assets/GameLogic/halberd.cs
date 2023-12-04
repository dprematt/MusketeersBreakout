using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Halberd : Weapons
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
    public void buildHalberd()
    {
        _Image = Resources.Load("Sprites/halberd") as Sprite;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.gameObject.transform;
        weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        IsPlayer = true;
    }
    public override void OnPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.transform;
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Halberd"; }
    }
    public void SpawnWeaponProx()
    {
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 4 / 2;
        NewPos.y += 1;
        var halberd = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        if (IsPlayer == true)
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
        else
        {
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4, true);
        }
    }

    public override void Attack()
    {
        SpawnWeaponProx();
    }

    public void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.L))
         {
             SpawnWeaponProx();
             if (UpdateLifeTime(LifeTime--))
                 Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
         }*/
    }
}
