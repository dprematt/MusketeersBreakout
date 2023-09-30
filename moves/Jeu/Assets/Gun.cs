using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponsRange
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        Damage = 5;
        Range = 5;
        Lifetime = 27;
    }

    public void SpawnBullet()
    {
        if (Lifetime > 0) {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(bulletSpawnPoint, Range, Damage);
        }
    }
}
