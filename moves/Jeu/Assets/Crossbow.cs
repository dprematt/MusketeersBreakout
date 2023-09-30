using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : WeaponsRange
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5;
    public int posInit = 0;
    public int posActual = 0;
    // Start is called before the first frame update
    void Start()
    {
        Damage = 8;
        Range = 5;
        Lifetime = 27;
    }

    public void SpawnBullet()
    {
        //Debug.Log(Lifetime);
        if (Lifetime > 0)
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        }
    }
}
