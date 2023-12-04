using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapons
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;
    public Sprite _Image = null;

    public override Sprite Image
    {
        get { return _Image; }
    }

    public void buildSword()
    {
        weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        _Image = Resources.Load("Sprites/sword") as Sprite;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.gameObject.transform;
        IsPlayer = true;
    }
    private void Start()
    {
       
        LifeTime = 10;
    }
    public override void OnPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.transform;
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Sword"; }
    }
    public void SpawnWeaponProx()
    { 
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 3/2;
        NewPos.y += 1;
        var sword = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        sword.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 3, 5);
    }

    public override void Attack()
    {
        SpawnWeaponProx();
    }
    public void Update()
    {
      /*  if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWeaponProx();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
        }*/
    }
}
