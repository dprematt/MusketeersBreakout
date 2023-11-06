using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : Weapons
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5;
    public Sprite _Image = null;

    private void Start()
    {
        LifeTime = 10;
    }
    public override Sprite Image
    {
        get { return _Image; }
    }

    public override void OnPickup()
    {
        gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "CrossBow"; }
    }
    public void SpawnBullet()
    {
        //Debug.Log(Lifetime);
        if (LifeTime > 0)
        {
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
                Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
        }
    }
}
