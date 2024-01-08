using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : Weapons
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    public Sprite _Image = null;

    public AudioClip shootingSound;
    private AudioSource audioSource;

    public override Sprite Image
    {
        get { return _Image; }
    }
    private void Start()
    {
        LifeTime = 10;
        audioSource = GetComponent<AudioSource>();
    }
    public override void OnPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0.3f, 1f, 0.0f);
        bulletSpawnPoint = player.transform;

        //gameObject.SetActive(false);
        //photonView.RPC("DisableObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    
    public override string Name
    {
        get { return "Gun"; }
    }

    public override void Attack()
    {
        audioSource.PlayOneShot(shootingSound);

        SpawnBullet();
        Debug.Log("Attack");
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
                Destroy(gameObject); // destruction de l'arc si la durabilit? atteint 0;
        }*/
    }
}
