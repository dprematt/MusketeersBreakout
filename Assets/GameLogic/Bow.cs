using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapons
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
        Debug.Log("Bow onPickup");
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Bow"; }
    }
    public void SpawnBullet()
    {
        if (LifeTime > 0) {
            var NewPos = bulletSpawnPoint.position;
            NewPos.z += 1;
            NewPos.y += 1;
            var bullet = Instantiate(bulletPrefab, NewPos, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(bulletSpawnPoint, Range, Damage);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnBullet();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arc si la durabilité atteint 0;
        }
    }
}
