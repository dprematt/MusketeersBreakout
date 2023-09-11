using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : WeaponsRange
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    public int posInit = 0;
    public int posActual = 0;
    // Start is called before the first frame update
    void Start()
    {
        Damage = 4;
        Range = 8;
        Lifetime = 15;
    }

    public void SpawnBullet()
    {
       //Debug.Log(Lifetime);
        if (Lifetime > 0) {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        }
    }
}
