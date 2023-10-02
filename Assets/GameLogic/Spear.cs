using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : WeaponsRange
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        Damage = 3;
        Range = 5;
        Lifetime = 27;
    }

    public void SpawnBullet()
    {
        //Debug.Log(Lifetime);
        if (Lifetime > 0)
        {
            var NewPos = bulletSpawnPoint.position;
            NewPos.z += 1;
            NewPos.y += 1;
            var bullet = Instantiate(bulletPrefab, NewPos, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(bulletSpawnPoint, Range, Damage);
        }
    }
}
