using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapons
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        bulletSpawnPoint = player.transform;
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Gun"; }
    }

    public override void Attack()
    {
        SpawnBullet();
    }
    public void SpawnBullet()
    {
        if (LifeTime > 0) {
            var NewPos = bulletSpawnPoint.position;
            NewPos.z += 1;
            NewPos.y += 1;
            Damage = 4;
            var bullet = Instantiate(bulletPrefab, NewPos, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(bulletSpawnPoint, Range, Damage);
        }
    }
    public void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnBullet();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arc si la durabilité atteint 0;
        }*/
    }
}
